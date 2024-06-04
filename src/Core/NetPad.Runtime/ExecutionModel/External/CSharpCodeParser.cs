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
        IEnumerable<string>? usings = null,
        CodeParsingOptions? options = null)
    {
        var bootstrapperProgramCode = SourceCode.Parse(GetEmbeddedBootstrapperProgram());

        var userProgramUsings = usings?.ToList() ?? new();

        if (options?.IncludeAspNetUsings == true)
        {
            userProgramUsings.AddRange(_aspNetUsings);
        }

        var userProgramCode = new SourceCode(GetUserProgram(code, scriptKind), userProgramUsings.ToHashSet());

        var additionalCode = options != null ? new SourceCodeCollection(options.AdditionalCode) : null;

        return new CodeParsingResult(userProgramCode, bootstrapperProgramCode, additionalCode);
    }

    private static string GetUserProgram(string scriptCode, ScriptKind kind)
    {
        string userProgram;

        if (kind == ScriptKind.Expression)
        {
            throw new NotImplementedException("Expression code parsing is not implemented yet.");
        }

        if (kind == ScriptKind.SQL)
        {
            scriptCode = scriptCode.Replace("\"", "\"\"");

            userProgram = GetEmbeddedSqlProgram().Replace("SQL_CODE", scriptCode);
        }
        else
        {
            userProgram = scriptCode;
        }

        return userProgram;
    }

    private static string GetEmbeddedBootstrapperProgram()
    {
        return AssemblyUtil.ReadEmbeddedResource(typeof(CSharpCodeParser).Assembly, "EmbeddedCode.Program.cs");
    }

    private static string GetEmbeddedSqlProgram()
    {
        return AssemblyUtil.ReadEmbeddedResource(typeof(CSharpCodeParser).Assembly, "EmbeddedCode.SqlAccessCode.cs");
    }
}
