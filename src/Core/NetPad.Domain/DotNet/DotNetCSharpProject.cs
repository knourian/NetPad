using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NetPad.Common;

namespace NetPad.DotNet;

/// <summary>
/// Represents a .NET C# project. Provides methods to create, delete and manage packages and assembly references.
/// </summary>
public class DotNetCSharpProject
{
    private readonly HashSet<Reference> _references;
    private readonly SemaphoreSlim _projectFileLock;

    /// <summary>
    /// Creates an instance of <see cref="DotNetCSharpProject"/>.
    /// </summary>
    /// <param name="projectDirectoryPath">Project root directory path.</param>
    /// <param name="projectFileName">If name of the project file. '.csproj' extension will be added if not specified.</param>
    /// <param name="packageCacheDirectoryPath">The package cache directory to use when adding or removing packages.
    /// Only needed when adding or removing package references.</param>
    public DotNetCSharpProject(string projectDirectoryPath, string projectFileName = "project.csproj", string? packageCacheDirectoryPath = null)
    {
        _references = new HashSet<Reference>();
        _projectFileLock = new SemaphoreSlim(1, 1);

        ProjectDirectoryPath = projectDirectoryPath;
        PackageCacheDirectoryPath = packageCacheDirectoryPath;

        if (!projectFileName.EndsWith(".csproj", StringComparison.OrdinalIgnoreCase))
            projectFileName += ".csproj";

        ProjectFilePath = Path.Combine(projectDirectoryPath, projectFileName);
    }

    /// <summary>
    /// The root directory of this project.
    /// </summary>
    public string ProjectDirectoryPath { get; }

    /// <summary>
    /// The path to the project file.
    /// </summary>
    public string ProjectFilePath { get; }

    /// <summary>
    /// The package cache directory to use when adding or removing packages.
    /// </summary>
    public string? PackageCacheDirectoryPath { get; }

    /// <summary>
    /// The references that are added to this project.
    /// </summary>
    public IReadOnlySet<Reference> References => _references;


    /// <summary>
    /// Creates the project on disk.
    /// </summary>
    /// <param name="outputType">The output type of the project.</param>
    /// <param name="deleteExisting">If true, will delete the project directory if it already exists on disk.</param>
    public virtual async Task CreateAsync(ProjectOutputType outputType, bool deleteExisting = false)
    {
        if (deleteExisting)
        {
            await DeleteAsync();
        }

        Directory.CreateDirectory(ProjectDirectoryPath);

        string dotnetOutputType = outputType == ProjectOutputType.Executable ? "Exe" : "Library";

        string xml = $@"<Project Sdk=""Microsoft.NET.Sdk"">

    <PropertyGroup>
        <OutputType>{dotnetOutputType}</OutputType>
        <TargetFramework>{BadGlobals.TargetFramework}</TargetFramework>
        <ImplicitUsings>enable</ImplicitUsings>
        <Nullable>enable</Nullable>
    </PropertyGroup>

</Project>
";

        await File.WriteAllTextAsync(ProjectFilePath, xml);
    }

    /// <summary>
    /// Deletes project directory on disk.
    /// </summary>
    public Task DeleteAsync()
    {
        if (Directory.Exists(ProjectDirectoryPath))
        {
            Directory.Delete(ProjectDirectoryPath, recursive: true);
        }

        return Task.CompletedTask;
    }


    public Task AddReferenceAsync(Reference reference)
    {
        if (reference is AssemblyReference assemblyReference)
        {
            return AddAssemblyReferenceAsync(assemblyReference);
        }
        else if (reference is PackageReference packageReference)
        {
            return AddPackageAsync(packageReference);
        }
        else
        {
            throw new InvalidOperationException($"Unhandled reference type.");
        }
    }

    public Task RemoveReferenceAsync(Reference reference)
    {
        if (reference is AssemblyReference assemblyReference)
        {
            return RemoveAssemblyReferenceAsync(assemblyReference);
        }
        else if (reference is PackageReference packageReference)
        {
            return RemovePackageAsync(packageReference);
        }
        else
        {
            throw new InvalidOperationException($"Unhandled reference type.");
        }
    }

    public async Task AddReferencesAsync(IEnumerable<Reference> references)
    {
        foreach (var reference in references)
        {
            await AddReferenceAsync(reference);
        }
    }

    public async Task RemoveReferencesAsync(IEnumerable<Reference> references)
    {
        foreach (var reference in references)
        {
            await RemoveReferenceAsync(reference);
        }
    }

    /// <summary>
    /// Adds an assembly reference to the project.
    /// </summary>
    /// <param name="reference">The assembly reference to add.</param>
    /// <exception cref="FormatException">Thrown if the project file XML is not formatted properly.</exception>
    public virtual async Task AddAssemblyReferenceAsync(AssemblyReference reference)
    {
        if (_references.Contains(reference))
        {
            return;
        }

        await _projectFileLock.WaitAsync();

        try
        {
            string assemblyPath = reference.AssemblyPath;

            var xmlDoc = XDocument.Load(ProjectFilePath);

            var root = xmlDoc.Elements("Project").FirstOrDefault();

            if (root == null)
            {
                throw new FormatException("Project XML file is not formatted correctly.");
            }

            // Check if it is already added
            if (FindAssemblyReferenceElement(assemblyPath, xmlDoc) != null)
            {
                return;
            }

            var referenceGroup = root.Elements("ItemGroup").FirstOrDefault(g => g.Elements("Reference").Any());

            if (referenceGroup == null)
            {
                referenceGroup = new XElement("ItemGroup");
                root.Add(referenceGroup);
            }

            var referenceElement = new XElement("Reference");

            referenceElement.SetAttributeValue("Include", AssemblyName.GetAssemblyName(assemblyPath).FullName);

            var hintPathElement = new XElement("HintPath");
            hintPathElement.SetValue(assemblyPath);
            referenceElement.Add(hintPathElement);

            referenceGroup.Add(referenceElement);

            await File.WriteAllTextAsync(ProjectFilePath, xmlDoc.ToString());

            _references.Add(reference);
        }
        finally
        {
            _projectFileLock.Release();
        }
    }

    /// <summary>
    /// Removes an assembly reference from the project.
    /// </summary>
    /// <param name="reference">The assembly reference to remove.</param>
    /// <exception cref="FormatException">Thrown if the project file XML is not formatted properly.</exception>
    public virtual async Task RemoveAssemblyReferenceAsync(AssemblyReference reference)
    {
        if (!_references.Contains(reference))
        {
            return;
        }

        await _projectFileLock.WaitAsync();

        try
        {
            var assemblyPath = reference.AssemblyPath;

            var xmlDoc = XDocument.Load(ProjectFilePath);

            var root = xmlDoc.Elements("Project").FirstOrDefault();

            if (root == null)
            {
                throw new FormatException("Project XML file is not formatted correctly.");
            }

            var referenceElementToRemove = FindAssemblyReferenceElement(assemblyPath, xmlDoc);

            if (referenceElementToRemove == null)
            {
                return;
            }

            referenceElementToRemove.Remove();

            await File.WriteAllTextAsync(ProjectFilePath, xmlDoc.ToString());

            _references.Remove(reference);
        }
        finally
        {
            _projectFileLock.Release();
        }
    }

    /// <summary>
    /// Adds a package reference to the project and installs it.
    /// </summary>
    /// <param name="reference">The package to add.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="PackageCacheDirectoryPath"/> is not set.</exception>
    public virtual async Task AddPackageAsync(PackageReference reference)
    {
        if (_references.Contains(reference))
        {
            return;
        }

        await _projectFileLock.WaitAsync();

        try
        {
            if (PackageCacheDirectoryPath == null)
                throw new InvalidOperationException($"{nameof(PackageCacheDirectoryPath)} is not set.");

            if (!Directory.Exists(PackageCacheDirectoryPath))
                throw new InvalidOperationException($"{nameof(PackageCacheDirectoryPath)} '{PackageCacheDirectoryPath}' does not exist.");

            var packageId = reference.PackageId;
            var packageVersion = reference.Version;

            var process = Process.Start(new ProcessStartInfo("dotnet",
                $"add \"{ProjectFilePath}\" package {packageId} " +
                $"--version {packageVersion} " +
                $"--package-directory \"{PackageCacheDirectoryPath}\"")
            {
                UseShellExecute = false,
                WorkingDirectory = ProjectDirectoryPath,
                CreateNoWindow = true
            });

            if (process != null)
            {
                await process.WaitForExitAsync();
            }
        }
        finally
        {
            _projectFileLock.Release();
        }
    }

    /// <summary>
    /// Removes a package reference from the project and uninstalls it.
    /// </summary>
    /// <param name="reference">The package to remove.</param>
    /// <exception cref="InvalidOperationException">Thrown if <see cref="PackageCacheDirectoryPath"/> is not set.</exception>
    public virtual async Task RemovePackageAsync(PackageReference reference)
    {
        if (!_references.Contains(reference))
        {
            return;
        }

        await _projectFileLock.WaitAsync();

        try
        {
            if (PackageCacheDirectoryPath == null)
                throw new InvalidOperationException($"{nameof(PackageCacheDirectoryPath)} is not set.");

            if (!Directory.Exists(PackageCacheDirectoryPath))
                throw new InvalidOperationException($"{nameof(PackageCacheDirectoryPath)} '{PackageCacheDirectoryPath}' does not exist.");

            var packageId = reference.PackageId;

            var process = Process.Start(new ProcessStartInfo("dotnet",
                $"remove \"{ProjectFilePath}\" package {packageId}")
            {
                UseShellExecute = false,
                WorkingDirectory = ProjectDirectoryPath,
                CreateNoWindow = true
            });

            if (process != null)
            {
                await process.WaitForExitAsync();
            }

            // This is needed so that 'project.assets.json' file is updated properly
            process = Process.Start(new ProcessStartInfo("dotnet",
                $"restore {ProjectFilePath}")
            {
                UseShellExecute = false,
                WorkingDirectory = ProjectDirectoryPath,
                CreateNoWindow = true
            });

            if (process != null)
            {
                await process.WaitForExitAsync();
            }
        }
        finally
        {
            _projectFileLock.Release();
        }
    }

    private XElement? FindAssemblyReferenceElement(string assemblyPath, XDocument xmlDoc)
    {
        var root = xmlDoc.Elements("Project").First();

        var itemGroups = root.Elements("ItemGroup");
        foreach (var itemGroup in itemGroups)
        {
            var assemblyReferenceElements = itemGroup.Elements("Reference");

            foreach (var assemblyReferenceElement in assemblyReferenceElements)
            {
                if (assemblyReferenceElement.Elements("HintPath").Any(hp => hp.Value == assemblyPath))
                {
                    return assemblyReferenceElement;
                }
            }
        }

        return null;
    }
}