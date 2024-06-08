using System.IO;
using System.Reflection;
using Microsoft.CodeAnalysis;
using NetPad.Common;
using NetPad.Compilation;
using NetPad.Configuration;
using NetPad.Data;
using NetPad.DotNet;
using NetPad.IO;
using NetPad.Packages;
using NetPad.Presentation;

namespace NetPad.ExecutionModel.External;

public partial class ExternalScriptRunner
{
    private readonly ICodeParser _codeParser;
    private readonly ICodeCompiler _codeCompiler;
    private readonly IPackageProvider _packageProvider;
    private readonly Settings _settings;

    private async Task<RunDependencies?> GetRunDependencies(RunOptions runOptions)
    {
        var references = new List<Reference>();
        var additionalCode = new SourceCodeCollection();

        // Add script references
        references.AddRange(_script.Config.References);

        // Add data connection resources
        if (_script.DataConnection != null)
        {
            var dcResources = await GetDataConnectionResourcesAsync(_script.DataConnection);

            if (dcResources.Code.Count > 0)
            {
                additionalCode.AddRange(dcResources.Code);
            }

            if (dcResources.References.Count > 0)
            {
                references.AddRange(dcResources.References);
            }
        }

        // Resolve all assembly images
        var images = references
            .Select(r => r is AssemblyImageReference air ? air.AssemblyImage : null!)
            .Where(r => r != null!)
            .ToList();


        // Resolve all assembly assets
        var referenceAssets = (
                await references.GetAssetsAsync(_script.Config.TargetFrameworkVersion, _packageProvider)
            )
            .Select(a => new
            {
                a.Path,
                IsAssembly = a.IsAssembly()
            })
            .DistinctBy(a => a.Path)
            .ToArray();

        // Get assembly file assets
        var assemblyFilePaths = referenceAssets
            .Where(a => a.IsAssembly)
            .Select(x => new
            {
                x.Path,
                AssemblyName = AssemblyName.GetAssemblyName(x.Path)
            })
            // Choose the highest version of duplicate assemblies
            .GroupBy(a => a.AssemblyName.Name)
            .Select(grp => grp.OrderBy(x => x.AssemblyName.Version).Last())
            .Select(x => x.Path)
            .ToHashSet();


        // Add assembly files needed to support running external process
        foreach (var assemblyPath in _supportAssemblies.Concat(_userVisibleAssemblies))
        {
            assemblyFilePaths.Add(assemblyPath);
        }

        // Parse Code & Compile
        var (parsingResult, compilationResult) = ParseAndCompile.Do(
            runOptions.SpecificCodeToRun ?? _script.Code,
            _script,
            _codeParser,
            _codeCompiler,
            images,
            assemblyFilePaths,
            additionalCode);

        if (!compilationResult.Success)
        {
            var error = compilationResult
                .Diagnostics
                .Where(d => d.Severity == DiagnosticSeverity.Error)
                .Select(d => CorrectDiagnosticErrorLineNumber(d, parsingResult.UserProgramStartLineNumber));

            await _output.WriteAsync(new ErrorScriptOutput(error.JoinToString("\n") + "\n"));

            return null;
        }

        // Get non-assembly file assets
        var fileAssets = referenceAssets
            .Where(a => !a.IsAssembly)
            .Select(a => new FileAssetCopy(a.Path, $"./{Path.GetFileName(a.Path)}"))
            .ToHashSet();

        return new RunDependencies(
            parsingResult,
            compilationResult.AssemblyBytes,
            images,
            assemblyFilePaths,
            fileAssets
        );
    }

    private async Task<(SourceCodeCollection Code, IReadOnlyList<Reference> References)>
        GetDataConnectionResourcesAsync(DataConnection dataConnection)
    {
        var code = new SourceCodeCollection();
        var references = new List<Reference>();

        var targetFrameworkVersion = _script.Config.TargetFrameworkVersion;

        var connectionCode = await _dataConnectionResourcesCache.GetSourceGeneratedCodeAsync(
            dataConnection,
            targetFrameworkVersion);

        if (connectionCode.ApplicationCode.Any())
        {
            code.AddRange(connectionCode.ApplicationCode);
        }

        var connectionAssembly = await _dataConnectionResourcesCache.GetAssemblyAsync(
            dataConnection,
            targetFrameworkVersion);

        if (connectionAssembly != null)
        {
            references.Add(new AssemblyImageReference(connectionAssembly));
        }

        var requiredReferences = await _dataConnectionResourcesCache.GetRequiredReferencesAsync(
            dataConnection,
            targetFrameworkVersion);

        if (requiredReferences.Any())
        {
            references.AddRange(requiredReferences);
        }

        return (code, references);
    }

    private async Task<FilePath> SetupExternalProcessRootDirectoryAsync(RunDependencies runDependencies)
    {
        // Create a new dir for each run
        _externalProcessRootDirectory.Refresh();

        if (_externalProcessRootDirectory.Exists)
        {
            _externalProcessRootDirectory.Delete(true);
        }

        _externalProcessRootDirectory.Create();

        // Write compiled assembly to dir
        var fileSafeScriptName = StringUtil
                                     .RemoveInvalidFileNameCharacters(_script.Name, "_")
                                     .Replace(" ", "_")
                                 // Arbitrary suffix so we don't match an assembly/asset with the same name.
                                 // Example: Assume user names script "Microsoft.Extensions.DependencyInjection"
                                 // If user also has a reference to "Microsoft.Extensions.DependencyInjection.dll"
                                 // then code further below will not copy the "Microsoft.Extensions.DependencyInjection.dll"
                                 // to the output directory, resulting in the referenced assembly not being found.
                                 + "__";

        FilePath scriptAssemblyFilePath =
            Path.Combine(_externalProcessRootDirectory.FullName, $"{fileSafeScriptName}.dll");

        await File.WriteAllBytesAsync(scriptAssemblyFilePath.Path, runDependencies.ScriptAssemblyBytes);

        // A runtimeconfig.json file tells .NET how to run the assembly
        await File.WriteAllTextAsync(
            Path.Combine(_externalProcessRootDirectory.FullName, $"{fileSafeScriptName}.runtimeconfig.json"),
            GenerateRuntimeConfigFileContents(runDependencies)
        );

        // The scriptconfig.json is custom and passes some options to the running script
        await File.WriteAllTextAsync(
            Path.Combine(_externalProcessRootDirectory.FullName, "scriptconfig.json"),
            $@"{{
    ""output"": {{
        ""maxDepth"": {_settings.Results.MaxSerializationDepth},
        ""maxCollectionSerializeLength"": {_settings.Results.MaxCollectionSerializeLength}
    }}
}}");

        foreach (var referenceAssemblyImage in runDependencies.AssemblyImageDependencies)
        {
            var fileName = referenceAssemblyImage.ConstructAssemblyFileName();

            await File.WriteAllBytesAsync(
                Path.Combine(_externalProcessRootDirectory.FullName, fileName),
                referenceAssemblyImage.Image);
        }

        foreach (var referenceAssemblyPath in runDependencies.AssemblyPathDependencies)
        {
            var destPath = Path.Combine(_externalProcessRootDirectory.FullName,
                Path.GetFileName(referenceAssemblyPath));

            // Checking file exists means that the first assembly in the list of paths will win.
            // Later assemblies with the same file name will not be copied to the output directory.
            if (!File.Exists(destPath))
                File.Copy(referenceAssemblyPath, destPath, true);
        }

        foreach (var asset in runDependencies.FileAssetsToCopy)
        {
            if (!asset.CopyFrom.Exists())
            {
                continue;
            }

            var copyTo = Path.GetFullPath(Path.Combine(_externalProcessRootDirectory.FullName, asset.CopyTo.Path));

            if (!copyTo.StartsWith(_externalProcessRootDirectory.FullName))
            {
                throw new Exception("Cannot copy asset to path outside the script start directory");
            }

            File.Copy(asset.CopyFrom.Path, copyTo, true);
        }

        return scriptAssemblyFilePath;
    }

    private string GenerateRuntimeConfigFileContents(RunDependencies runDependencies)
    {
        var runtimeVersion = _dotNetInfo.GetDotNetRuntimeVersionsOrThrow()
            .Where(v =>
                v.FrameworkName == "Microsoft.NETCore.App"
                && v.Version.Major == _script.Config.TargetFrameworkVersion.GetMajorVersion())
            .MaxBy(v => v.Version)?
            .Version;

        if (runtimeVersion == null)
            throw new Exception(
                $"Could not find a .NET {_script.Config.TargetFrameworkVersion.GetMajorVersion()} runtime");

        var tfm = _script.Config.TargetFrameworkVersion.GetTargetFrameworkMoniker();
        var probingPaths =
            JsonSerializer.Serialize(runDependencies.AssemblyPathDependencies.Select(Path.GetDirectoryName).Distinct());

        return $@"{{
    ""runtimeOptions"": {{
        ""tfm"": ""{tfm}"",
        ""framework"": {{
            ""name"": ""Microsoft.AspNetCore.App"",
            ""version"": ""{runtimeVersion}""
        }},
        ""rollForward"": ""Minor"",
        ""additionalProbingPaths"": {probingPaths}
    }}
}}";
    }

    /// <summary>
    /// Corrects line numbers in compilation errors relative to the line number where user code starts.
    /// </summary>
    private static string CorrectDiagnosticErrorLineNumber(Diagnostic diagnostic, int userProgramStartLineNumber)
    {
        var err = diagnostic.ToString();

        if (!err.StartsWith('('))
        {
            return err;
        }

        var errParts = err.Split(':');
        var span = errParts.First().Trim(new[] { '(', ')' });
        var spanParts = span.Split(',');
        var lineNumberStr = spanParts[0];

        return int.TryParse(lineNumberStr, out int lineNumber)
            ? $"({lineNumber - userProgramStartLineNumber},{spanParts[1]}):{errParts.Skip(1).JoinToString(":")}"
            : err;
    }

    private record RunDependencies(
        CodeParsingResult ParsingResult,
        byte[] ScriptAssemblyBytes,
        List<AssemblyImage> AssemblyImageDependencies,
        HashSet<string> AssemblyPathDependencies,
        HashSet<FileAssetCopy> FileAssetsToCopy);
}
