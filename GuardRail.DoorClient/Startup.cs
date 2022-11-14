using System;
using System.Device.Gpio;
using System.Net.Http.Headers;
using GuardRail.DeviceLogic.DependencyHelpers;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Models;
using GuardRail.DeviceLogic.Services;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Implementation;
using GuardRail.DoorClient.Implementation.Communication;
using GuardRail.DoorClient.Implementation.Feedback.Buzzer;
using GuardRail.DoorClient.Implementation.Feedback.Lights;
using GuardRail.DoorClient.Implementation.Input;
using GuardRail.DoorClient.Implementation.Input.Nfc;
using GuardRail.DoorClient.Interfaces;
using GuardRail.DoorClient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Exceptions;

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
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger();
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
            .AddLogging(logging =>
            {
                logging.ClearProviders();
                logging.AddSerilog();
            })
            .AddSingleton(_ => Configuration.GetSection(nameof(UdpConfiguration)).Get<UdpConfiguration>() ?? new UdpConfiguration())
            .AddSingleton<IUdpConfiguration, UdpConfiguration>()
            .AddSingleton(_ => new GpioController())
            .AddSingleton<IGpio, Gpio>()
            .AddBuzzer<BuzzerConfiguration, int, BuzzerHardwareManager, BuzzerManager>(Configuration)
            .AddLight<LightConfiguration, int, LightHardwareManager, LightManager>(Configuration)
            .AddKeypad<KeypadConfiguration, int, KeypadHardwareManager, KeypadInput>(Configuration)
            //.AddNfc<NfcConfiguration, NfcHardwareManager, NfcInput>(Configuration)
            .AddEmptyNfc()
            .AddEmptyScreen()
            .AddEmptyDoor()
            .AddSingleton<ICentralServerCommunication, CentralServerCommunication>()
            .AddSingleton<ICentralServerPushCommunication, CentralServerPushCommunication>()
            .AddHostedService<ButtonListenerService>()
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
        app.UseAsyncInitTypes();
    }
}