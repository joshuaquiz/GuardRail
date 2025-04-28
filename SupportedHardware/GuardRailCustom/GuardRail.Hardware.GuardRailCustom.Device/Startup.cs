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
using System.Net.Sockets;

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
            .AddSingleton(
                new HardwareDiscoveryPacket
                {
                    Port = new IPEndPoint(IPAddress.Loopback, 0).Port,
                    IpAddress = Dns.GetHostEntry(
                                        Dns.GetHostName())
                                    .AddressList
                                    .FirstOrDefault(x =>
                                        x.AddressFamily == AddressFamily.InterNetwork)
                                ?? IPAddress.Parse(
                                    "127.0.0.1"),
                    Name = Dns.GetHostName(),
                    IsRunningOnDevice = false
                })
            .AddSingleton<GuardRailUdpClientFactory>()
            .AddKeyedSingleton<IUdpCommandHandler, UnLockDoorUdpCommandHandler>(UnLockDoorUdpCommandHandler.CommandName)
            .AddHostedService<DeviceHardwareUdpDiscoveryListenerBackgroundWorker>()
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