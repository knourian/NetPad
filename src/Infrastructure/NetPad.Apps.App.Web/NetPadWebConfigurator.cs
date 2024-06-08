using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPad.Application;
using NetPad.Apps.App.Common.Application;
using NetPad.Apps.App.Common.UiInterop;
using NetPad.Web.UiInterop;

namespace NetPad.Web;

public class NetPadWebConfigurator : IApplicationConfigurator
{
    public void ConfigureWebHost(IWebHostBuilder webHostBuilder, string[] programArgs)
    {
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(new ApplicationInfo(ApplicationType.Web));

        services.AddTransient<IUiWindowService, WebWindowService>();
        services.AddTransient<IUiDialogService, WebDialogService>();
        services.AddTransient<IIpcService, SignalRIpcService>();
    }

    public void Configure(IApplicationBuilder app, IHostEnvironment env)
    {
    }

    public void ShowErrorDialog(string title, string content)
    {
        // Do nothing. Not supported on this platform
    }
}
