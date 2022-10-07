using System;
using System.Device.Gpio;
using System.Linq;
using System.Net.Http.Headers;
using System.Reflection;
using GuardRail.DeviceLogic.Interfaces;
using GuardRail.DeviceLogic.Interfaces.Communication;
using GuardRail.DeviceLogic.Interfaces.Feedback.Buzzer;
using GuardRail.DeviceLogic.Interfaces.Feedback.Lights;
using GuardRail.DeviceLogic.Interfaces.Input.Keypad;
using GuardRail.DeviceLogic.Models;
using GuardRail.DeviceLogic.Services;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Implementation;
using GuardRail.DoorClient.Implementation.Communication;
using GuardRail.DoorClient.Implementation.Feedback.Buzzer;
using GuardRail.DoorClient.Implementation.Feedback.Lights;
using GuardRail.DoorClient.Implementation.Input;
using GuardRail.DoorClient.Interfaces;
using GuardRail.DoorClient.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

namespace GuardRail.DoorClient;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
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
        RegisterAllImplementations(services, typeof(IHardwareAsyncInit));
        services
            .AddOptions()
            .AddLogging()
            .AddSingleton(_ => Configuration.GetSection(nameof(BuzzerConfiguration)).Get<BuzzerConfiguration>())
            .AddSingleton(_ => Configuration.GetSection(nameof(LightConfiguration)).Get<LightConfiguration>())
            .AddSingleton(_ => Configuration.GetSection(nameof(KeypadConfiguration)).Get<KeypadConfiguration>())
            .AddSingleton(_ => Configuration.GetSection(nameof(UdpConfiguration)).Get<UdpConfiguration>())
            .AddSingleton(_ => new GpioController())
            .AddSingleton<IGpio, Gpio>()
            .AddSingleton<ILightHardwareManager, LightHardwareManager>()
            .AddSingleton<IBuzzerHardwareManager, BuzzerHardwareManager>()
            .AddSingleton<ILightManager, LightManager>()
            .AddSingleton<IBuzzerManager, BuzzerManager>()
            .AddSingleton<IKeypadHardwareManager, KeypadHardwareManager>()
            .AddHostedService<ButtonListenerService>()
            .AddSingleton<IKeypadInput, KeypadInput>()
            .AddSingleton<ICentralServerCommunication, CentralServerCommunication>()
            .AddSingleton<ICentralServerPushCommunication, CentralServerPushCommunication>()
            .AddHostedService<HardwareInitService>()
            .AddHostedService<UdpConnectionManagerService>()
            .AddHostedService<CentralServerPushCommunication>()
            .AddHostedService<LockService>()
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

    private static void RegisterAllImplementations(
        IServiceCollection serviceCollection,
        Type type)
    {
        foreach (var t in Assembly
                     .GetExecutingAssembly()
                     .DefinedTypes
                     .Where(x => type.IsAssignableFrom(x) && type != x.AsType()))
        {
            serviceCollection.AddSingleton(type, t.AsType());
        }
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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
    }
}