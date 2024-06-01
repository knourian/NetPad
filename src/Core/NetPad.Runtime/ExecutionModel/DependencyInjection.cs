using Microsoft.Extensions.DependencyInjection;
using NetPad.Compilation;
using NetPad.ExecutionModel.External;
using NetPad.ExecutionModel.InMemory;
using CSharpCodeParser = NetPad.ExecutionModel.External.CSharpCodeParser;

namespace NetPad.ExecutionModel;

public static class DependencyInjection
{
    /// <summary>
    /// Registers the "External" execution model. Only one execution model should be registered per-application.
    /// </summary>
    public static void AddExternalExecutionModel(this IServiceCollection services)
    {
        services.AddTransient<IScriptRunnerFactory, ExternalScriptRunnerFactory>();
        services.AddTransient<ICodeParser, CSharpCodeParser>();
    }

    /// <summary>
    /// Registers the "In Memory" execution model. Only one execution model should be registered per-application.
    /// </summary>
    public static void AddInMemoryExecutionModel(this IServiceCollection services)
    {
        services.AddTransient<IScriptRunnerFactory, InMemoryScriptRunnerFactory>();
        services.AddTransient<ICodeParser, InMemory.CSharpCodeParser>();
    }
}
