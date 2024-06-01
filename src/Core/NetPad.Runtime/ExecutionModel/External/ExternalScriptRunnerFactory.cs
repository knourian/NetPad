using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NetPad.Compilation;
using NetPad.Configuration;
using NetPad.DotNet;
using NetPad.Packages;
using NetPad.Scripts;

namespace NetPad.ExecutionModel.External;

public class ExternalScriptRunnerFactory : IScriptRunnerFactory
{
    private readonly IServiceProvider _serviceProvider;

    public ExternalScriptRunnerFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IScriptRunner CreateRunner(Script script)
    {
        return new ExternalScriptRunner(
            script,
            _serviceProvider.GetRequiredService<ICodeParser>(),
            _serviceProvider.GetRequiredService<ICodeCompiler>(),
            _serviceProvider.GetRequiredService<IPackageProvider>(),
            _serviceProvider.GetRequiredService<IDotNetInfo>(),
            _serviceProvider.GetRequiredService<Settings>(),
            _serviceProvider.GetRequiredService<ILogger<ExternalScriptRunner>>()
        );
    }
}
