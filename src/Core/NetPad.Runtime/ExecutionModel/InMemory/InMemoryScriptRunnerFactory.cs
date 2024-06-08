using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPad.Compilation;
using NetPad.Packages;
using NetPad.Scripts;

namespace NetPad.ExecutionModel.InMemory;

public class InMemoryScriptRunnerFactory(IServiceProvider serviceProvider) : IScriptRunnerFactory
{
    public IScriptRunner CreateRunner(Script script)
    {
        return new InMemoryScriptRunner(
            script,
            serviceProvider.CreateScope(),
            serviceProvider.GetRequiredService<ICodeParser>(),
            serviceProvider.GetRequiredService<ICodeCompiler>(),
            serviceProvider.GetRequiredService<IPackageProvider>(),
            serviceProvider.GetRequiredService<ILogger<InMemoryScriptRunner>>()
        );
    }
}
