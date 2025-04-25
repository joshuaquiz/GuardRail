using System.Linq;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.DependencyHelpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Net;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;
using GuardRail.Hardware.GuardRailCustom.Device.CommandHandlers;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public class Startup(
    IConfiguration configuration)
{
    public void ConfigureServices(
        IServiceCollection services)
    {
        DeviceConstants.DeviceId = Dns.GetHostName();
        services.AddLogging(
            x => x.AddConsole());
        services
            .AddOptions()
            .AddLogging()
            .AddSingleton<GuardRailUdpClientFactory>()
            .AddKeyedSingleton<IUdpCommandHandler, SyncUdpCommandHandler>(SyncUdpCommandHandler.CommandName)
            .AddHostedService<HardwareUdpDiscoveryListenerBackgroundWorker>()
            .AddGuardRailIntegratedHardware(configuration);
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        var inits = app.ApplicationServices.GetServices<IAsyncInit>().ToList();
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
        logger.LogDebug($"Initializing items ({inits.Count})");
        Task.WhenAll(
            inits
                .Select(
                    async x =>
                    {
                        logger.LogDebug($"Initializing {x.GetType().Name}");
                        await x.InitAsync();
                    })).GetAwaiter().GetResult();
        logger.LogDebug("Done with initializing");
    }
}