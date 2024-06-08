using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NetPad.Application;
using NetPad.Apps.App.Common.Application;
using NetPad.Configuration;
using NetPad.Electron;
using NetPad.Web;
using Serilog;

namespace NetPad;

public static class Program
{
    internal static IApplicationConfigurator ApplicationConfigurator { get; private set; } = null!;

    public static int Main(string[] args)
    {
        try
        {
            // Configure as an Electron app or a web app
            ApplicationConfigurator = args.Any(a => a.ContainsIgnoreCase("/ELECTRONPORT"))
                ? new NetPadElectronConfigurator()
                : new NetPadWebConfigurator();

            var host = CreateHostBuilder(args).Build();

            Console.WriteLine($"Starting NetPad. App type: {host.Services.GetRequiredService<ApplicationInfo>().Type}");

            host.Run();
            return 0;
        }
        catch (IOException ioException) when (ioException.Message.ContainsIgnoreCase("address already in use"))
        {
            Console.WriteLine($"Another instance is already running. {ioException.Message}");
            ApplicationConfigurator.ShowErrorDialog(
                $"{AppIdentifier.AppName} Already Running",
                $"{AppIdentifier.AppName} is already running. You cannot open multiple instances of {AppIdentifier.AppName}.");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Host terminated unexpectedly with error:\n{ex}");
            Log.Fatal(ex, "Host terminated unexpectedly");
            return 1;
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration(config => { config.AddJsonFile("appsettings.Local.json", true); })
            .UseSerilog((ctx, config) => { ConfigureLogging(config, ctx.Configuration); })
            .ConfigureWebHostDefaults(webBuilder =>
            {
                ApplicationConfigurator.ConfigureWebHost(webBuilder, args);
                webBuilder.UseStartup<Startup>();
            });

    private static void ConfigureLogging(LoggerConfiguration serilogConfig, IConfiguration appConfig)
    {
        Environment.SetEnvironmentVariable("NETPAD_LOG_DIR", AppDataProvider.LogDirectoryPath.Path);

        serilogConfig.ReadFrom.Configuration(appConfig);
    }
}
