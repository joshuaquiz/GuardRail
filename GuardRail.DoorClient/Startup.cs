using System.Device.Gpio;
using GuardRail.DoorClient.Configuration;
using GuardRail.DoorClient.Implementation;
using GuardRail.DoorClient.Interfaces;
using GuardRail.DoorClient.Logic;
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
        services
            .AddOptions()
            .AddLogging()
            .AddSingleton(_ => Configuration.GetSection(nameof(BuzzerConfiguration)).Get<BuzzerConfiguration>())
            .AddSingleton(_ => Configuration.GetSection(nameof(KeypadConfiguration)).Get<KeypadConfiguration>())
            .AddSingleton(_ => Configuration.GetSection(nameof(LightConfiguration)).Get<LightConfiguration>())
            .AddSingleton(_ => Configuration.GetSection(nameof(UdpConfiguration)).Get<UdpConfiguration>())
            .AddSingleton(_ => new GpioController())
            .AddSingleton<IGpio, Gpio>()
            .AddSingleton<ILightManager, LightManager>()
            .AddSingleton<IBuzzerManager, BuzzerManager>()
            .AddSingleton<IUdpSenderReceiver, UdpSenderReceiverReceiver>()
            .AddSingleton<IKeypadLogic, KeypadLogic>()
            .AddSingleton<IUdpWrapper, UdpWrapper>()
            .AddHostedService<ButtonListenerService>()
            .AddHostedService<NetworkConnectionCheckingService>()
            .AddHostedService<LockService>();
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