using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using GuardRail.Core.Helpers;
using GuardRail.Hardware.GuardRailCustom.Core;
using GuardRail.Hardware.GuardRailCustom.Device.BackgroundServices;
using GuardRail.Hardware.GuardRailCustom.Device.CommandHandlers;
using GuardRail.Hardware.GuardRailCustom.Device.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GuardRail.Hardware.GuardRailCustom.Device;

public class Startup(
    IConfiguration configuration)
{
    public void ConfigureServices(
        IServiceCollection services)
    {
        DeviceConstants.DeviceId = Dns.GetHostName();
        services
            .AddSingleton(
                new HardwareDiscoveryPacket
                {
                    EncryptionKey = Guid.NewGuid().ToString(),
                    Port = GetAvailablePort(),
                    IpAddress = Dns.GetHostEntry(
                                        Dns.GetHostName())
                                    .AddressList
                                    .FirstOrDefault(x =>
                                        x.AddressFamily == AddressFamily.InterNetwork
                                        && !x.ToString().StartsWith("127"))
                                ?? IPAddress.Parse(
                                    "0.0.0.0"),
                    Name = Dns.GetHostName(),
                    IsRunningOnDevice = true
                })
            .AddLogging(
                x =>
                {
                    x.ClearProviders();
                    x.SetMinimumLevel(LogLevel.Trace);
                    x.AddSimpleConsole(
                        c =>
                        {
                            c.TimestampFormat = "yyyy-MM-ddTHH:mm:ss ";
                            c.IncludeScopes = true;
                            c.SingleLine = true;
                            c.UseUtcTimestamp = true;
                        });
                })
            .AddOptions()
            .AddSingleton<GuardRailUdpClientFactory>()
            .AddKeyedSingleton<IUdpCommandHandler, UnLockDoorUdpCommandHandler>(UnLockDoorUdpCommandHandler.CommandName)
            .AddHostedService<DeviceHardwareUdpDiscoveryBroadcasterBackgroundWorker>()
            .AddGuardRailIntegratedHardware(configuration);
    }

    private static int GetAvailablePort()
    {
        var listener = new TcpListener(IPAddress.Any, 0);
        listener.Start();
        var port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        var logger = app.ApplicationServices.GetRequiredService<ILogger<Startup>>();
        logger.LogGuardRailInformation("Configuring Application");
        var inits = app.ApplicationServices.GetServices<IAsyncInit>().ToList();
        logger.LogGuardRailDebug($"Initializing items ({inits.Count})");
        Task.WhenAll(
            inits
                .Select(
                    async x =>
                    {
                        logger.LogGuardRailDebug($"Initializing {x.GetType().Name}");
                        await x.InitAsync();
                    }))
            .GetAwaiter()
            .GetResult();
        logger.LogGuardRailDebug("Done with initializing");
    }
}