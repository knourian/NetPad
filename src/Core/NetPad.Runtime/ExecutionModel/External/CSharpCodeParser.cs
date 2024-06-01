using NetPad.Compilation;
using NetPad.DotNet;
using NetPad.Scripts;

namespace NetPad.ExecutionModel.External;

public class CSharpCodeParser : ICodeParser
{
    private static readonly string[] _aspNetUsings = {
        "System.Net.Http.Json",
        "Microsoft.AspNetCore.Builder",
        "Microsoft.AspNetCore.Hosting",
        "Microsoft.AspNetCore.Http",
        "Microsoft.AspNetCore.Routing",
        "Microsoft.Extensions.Configuration",
        "Microsoft.Extensions.DependencyInjection",
        "Microsoft.Extensions.Hosting",
        "Microsoft.Extensions.Logging",
    };

    public CodeParsingResult Parse(
        string code,
        ScriptKind scriptKind,
        IEnumerable<string>? namespaces = null,
        CodeParsingOptions? options = null)
    {
        var userProgram = GetUserProgram(code, scriptKind);

        var bootstrapperProgram = GetBootstrapperProgram();

        var bootstrapperProgramSourceCode = SourceCode.Parse(bootstrapperProgram);

        var additionalCode = options != null ? new SourceCodeCollection(options.AdditionalCode) : new SourceCodeCollection();

        var usings = new List<string>();

        if (namespaces != null)
        {
            usings.AddRange(namespaces);
        }

        if (options?.IncludeAspNetUsings == true)
        {
            usings.AddRange(_aspNetUsings);
        }

        return new CodeParsingResult(new SourceCode(userProgram, usings), bootstrapperProgramSourceCode, additionalCode);
    }

    private static string GetUserProgram(string scriptCode, ScriptKind kind)
    {
        string userCode;

        if (kind == ScriptKind.Expression)
        {
            throw new NotImplementedException("Expression code parsing is not implemented yet.");
        }

        if (kind == ScriptKind.SQL)
        {
            scriptCode = scriptCode.Replace("\"", "\"\"");

            userCode = AssemblyUtil.ReadEmbeddedResource(typeof(CSharpCodeParser).Assembly, "EmbeddedCode.SqlAccessCode.cs")
                .Replace("SQL_CODE", scriptCode);
        }
        else
        {
            userCode = scriptCode;
        }

        return userCode;
    }

    private static string GetBootstrapperProgram()
    {
        return AssemblyUtil.ReadEmbeddedResource(typeof(CSharpCodeParser).Assembly, "EmbeddedCode.Program.cs");
    }
}
