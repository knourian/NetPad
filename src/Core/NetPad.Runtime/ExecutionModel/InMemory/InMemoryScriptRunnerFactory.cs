using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPad.Compilation;
using NetPad.Packages;
using NetPad.Scripts;

namespace NetPad.ExecutionModel.InMemory;

public class InMemoryScriptRunnerFactory : IScriptRunnerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public InMemoryScriptRunnerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IScriptRunner CreateRunner(Script script)
    {
        return new InMemoryScriptRunner(
            script,
            _serviceProvider.CreateScope(),
            _serviceProvider.GetRequiredService<ICodeParser>(),
            _serviceProvider.GetRequiredService<ICodeCompiler>(),
            _serviceProvider.GetRequiredService<IPackageProvider>(),
            _serviceProvider.GetRequiredService<ILogger<InMemoryScriptRunner>>()
        );
    }
}
