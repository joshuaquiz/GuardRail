using System;
using System.Linq;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.DependencyHelpers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using GuardRail.Hardware.GuardRailCustom.Device.Models;
using GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using GuardRail.Hardware.GuardRailCustom.Device.Feedback.Buzzer;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public class Startup(
    IConfiguration configuration)
{
    public void ConfigureServices(IServiceCollection services)
    {
        DeviceConstants.DeviceId = configuration["Name"] ?? $"Not-configured-{Guid.NewGuid()}";
        services.AddLogging(
            x => x.AddConsole());
        services
            .AddOptions()
            .AddLogging()
            .AddGuardRailIntegratedHardware(configuration)
            .AddSingleton<IAsyncInit, BuzzerManager>()
            /*.AddSingleton<ICentralServerCommunication, CentralServerCommunication>()
            .AddSingleton<ICentralServerPushCommunication, CentralServerPushCommunication>()*/
            .AddHostedService<HardwareUdpDiscoveryListenerBackgroundWorker>()
            /*.AddHostedService<CentralServerPushCommunication>()*/;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        /*app.UseCoreEventHandlers();*/
        var inits = app.ApplicationServices.GetServices<IAsyncInit>().ToList();
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
        logger.LogInformation($"Initializing stuff ({inits.Count})");
        Task.WhenAll(
            inits
                .Select(
                    async x =>
                    {
                        logger.LogInformation($"Initializing {x.GetType().Name}");
                        await x.InitAsync();
                    })).GetAwaiter().GetResult();
        logger.LogInformation("Done with initializing");
    }
}