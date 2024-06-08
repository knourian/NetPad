using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPad.Application;
using NetPad.Apps.Shells.Web.UiInterop;
using NetPad.Apps.UiInterop;

namespace NetPad.Apps.Shells.Web;

public class WebBrowserShell : IShell
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

    public void Initialize(IApplicationBuilder app, IHostEnvironment env)
    {
        // Do nothing.
    }

    public void ShowErrorDialog(string title, string content)
    {
        // Do nothing. Not supported on this platform
    }
}
