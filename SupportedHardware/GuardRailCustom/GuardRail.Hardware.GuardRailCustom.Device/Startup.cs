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
            x =>
            {
                x.ClearProviders();
                x.AddConsole();
                x.SetMinimumLevel(LogLevel.Trace);
            });
        services
            .AddOptions()
            .AddLogging()
            .AddSingleton(
                new HardwareDiscoveryPacket
                {
                    Port = GetAvailablePort(),
                    IpAddress = Dns.GetHostEntry(
                                        Dns.GetHostName())
                                    .AddressList
                                    .FirstOrDefault(x =>
                                        x.AddressFamily == AddressFamily.InterNetwork
                                        && !x.ToString().StartsWith("127"))
                                ?? IPAddress.Parse(
                                    "127.0.0.1"),
                    Name = Dns.GetHostName(),
                    IsRunningOnDevice = true
                })
            .AddSingleton<GuardRailUdpClientFactory>()
            .AddKeyedSingleton<IUdpCommandHandler, ConnectUdpCommandHandler>(ConnectUdpCommandHandler.CommandName)
            .AddKeyedSingleton<IUdpCommandHandler, UnLockDoorUdpCommandHandler>(UnLockDoorUdpCommandHandler.CommandName)
            .AddHostedService<UdpListenerBackgroundWorker>()
            .AddHostedService<DeviceHardwareUdpDiscoveryBroadcasterBackgroundWorker>()
            .AddGuardRailIntegratedHardware(configuration);
    }

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        var inits = app.ApplicationServices.GetServices<IAsyncInit>().ToList();
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
        var hwp = app.ApplicationServices.GetRequiredService<HardwareDiscoveryPacket>();
        logger.LogDebug($"Initializing items ({inits.Count})");
        logger.LogDebug($"IP config: {hwp.IpAddress}. Port: {hwp.Port}");
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