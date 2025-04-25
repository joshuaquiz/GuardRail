using System;
using System.Net.Http;
using System.Threading.Tasks;
using GuardRail.Api;
using GuardRail.Local.Service.BackgroundServices;
using GuardRail.Logic.Helpers;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace GuardRail.Local.Service;

public static class Program
{
    public static async Task Main(
        string[] args)
    {
        var builder = WebApplication.CreateBuilder(
            args);
        builder.Configuration.AddJsonFile("appsettings.json", false, true);
        if (builder.Environment.IsDevelopment())
        {
            builder.Configuration.AddJsonFile("appsettings.Development.json", true, true);
            builder.Configuration.AddJsonFile("appsettings.local.json", true, true);
        }

        //builder.Services.AddWindowsService();
#if DEBUG
        builder.Services
            .AddSingleton<HttpOverrides.DevelopmentHttpMessageHandlerOverride>()
            .AddSingleton(
                serviceProvider =>
                    new HttpClient(
                        serviceProvider.GetRequiredService<HttpOverrides.DevelopmentHttpMessageHandlerOverride>())
                    {
                        BaseAddress = new Uri(
                            builder.Configuration["WebHost"] ?? string.Empty,
                            UriKind.Absolute)
                    });
#else
        builder.Services.AddHttpClient(
            x =>
                x.BaseAddress = new Uri(
                    builder.Configuration["WebHost"] ?? string.Empty,
                    UriKind.Absolute));
#endif
        builder.Services.AddHostedService<AutoUpdateCheckerBackgroundService>();
        builder.Services.AddHostedService(
            serviceProvider =>
                new CommandProcessorBackgroundService(
                    Guid.NewGuid(),
                    serviceProvider.GetRequiredService<HttpClient>(),
                    serviceProvider,
                    serviceProvider.GetRequiredService<ILogger<CommandProcessorBackgroundService>>()));
        //builder.Services.AddHostedService<UdpPingListenerBackgroundService>();

        builder.Services.AddGuardRailCommandHandlers();
        builder.Services.AddGuardRailSupportedHardware();
        builder.Services.AddGuardRailApi();
        if (bool.Parse(builder.Configuration["IsAirGapped"] ?? "false"))
        {
            builder.Services.AddGuardRailLocalServices();
        }

        var app = builder.Build();
        app.UseHttpsRedirection();
        app.UseGuardRailApi();
        await app.RunAsync();
    }
}