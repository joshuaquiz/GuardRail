using System;
using System.Linq;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using GuardRail.DeviceLogic.Hardware.GuardRailIntegrated.DependencyHelpers;
using GuardRail.DeviceLogic.Interfaces;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Logic;
using GuardRail.DeviceLogic.Models;
using GuardRail.DeviceLogic.Services;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Implementation.Communication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace GuardRail.DoorClient;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddLogging(
            x => x.AddConsole());
        services.AddControllers();
        services.AddHealthChecks();
        services.AddSwaggerGen(c =>
            c.SwaggerDoc(
                "v1",
                new OpenApiInfo
                {
                    Title = "GuardRail.DoorClient",
                    Version = "v1"
                }));
        services
            .AddOptions()
            .AddLogging()
            .AddSingleton(_ => Configuration.GetSection(nameof(UdpConfiguration)).Get<UdpConfiguration>() ?? new UdpConfiguration())
            .AddSingleton<IUdpConfiguration, UdpConfiguration>()
            .AddGuardRailIntegratedHardware(Configuration)
            .AddSingleton<ICentralServerCommunication, CentralServerCommunication>()
            .AddSingleton<ICentralServerPushCommunication, CentralServerPushCommunication>()
            .AddHostedService<UdpConnectionManagerService>()
            .AddHostedService<CentralServerPushCommunication>()
            .AddHttpClient(
                "GuardRail",
                (_, client) =>
                {
                    client.BaseAddress = DeviceConstants.RemoteHostIpAddress != null
                        ? new Uri($"https://{DeviceConstants.RemoteHostIpAddress}/", UriKind.Absolute)
                        : new Uri("https://localhost/", UriKind.Absolute);
                    client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("GuardRailDoor", "1.0.0"));
                });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GuardRail.DoorClient v1"));
        }

        app.UseHttpsRedirection();
        app.UseRouting();
        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapHealthChecks(
                "/healthz",
                new HealthCheckOptions
                {
                    AllowCachingResponses = false
                });
        });

        app.UseCoreEventHandlers();
        var inits = app.ApplicationServices.GetServices<IAsyncInit>();
        Task.WhenAll(inits.Select(async x => await x.InitAsync())).GetAwaiter().GetResult();
    }
}