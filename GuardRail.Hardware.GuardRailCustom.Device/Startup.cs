using System.Linq;
using System.Threading.Tasks;
using GuardRail.Hardware.GuardRailCustom.Device.DependencyHelpers;
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
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(
            x => x.AddConsole());
        services
            .AddOptions()
            .AddLogging()
            .AddGuardRailIntegratedHardware(configuration)
            /*.AddSingleton<ICentralServerCommunication, CentralServerCommunication>()
            .AddSingleton<ICentralServerPushCommunication, CentralServerPushCommunication>()*/
            .AddHostedService<HardwareUdpDiscoveryBackgroundWorker>()
            /*.AddHostedService<CentralServerPushCommunication>()*/;
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        /*app.UseCoreEventHandlers();*/
        var inits = app.ApplicationServices.GetServices<IAsyncInit>();
        Task.WhenAll(inits.Select(async x => await x.InitAsync())).GetAwaiter().GetResult();
    }
}