using System;
using System.Net.Http;
using System.Threading.Tasks;
using GuardRail.Core.Enums;
using GuardRail.Logic.Commands.Implementations;
using GuardRail.Logic.Commands.Interfaces;
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
            .AddSingleton<DevelopmentHttpMessageHandlerOverride>()
            .AddSingleton(
                serviceProvider =>
                    new HttpClient(
                        serviceProvider.GetRequiredService<DevelopmentHttpMessageHandlerOverride>())
                    {
                        BaseAddress = new Uri(
                            "https://lol.fake.com",
                            UriKind.Absolute)
                    });
#else
        builder.Services.AddHttpClient(
            x =>
                x.BaseAddress = new Uri(
                    "https://lol.fake.com",
                    UriKind.Absolute));
#endif
        //builder.Services.AddHostedService<AutoUpdateCheckerWorker>();
        builder.Services.AddHostedService(
            serviceProvider =>
                new CommandProcessorBackgroundService(
                    Guid.NewGuid(),
                    serviceProvider.GetRequiredService<HttpClient>(),
                    serviceProvider,
                    serviceProvider.GetRequiredService<ILogger<CommandProcessorBackgroundService>>()));
        //builder.Services.AddHostedService<UdpPingListenerWorker>();
        builder.Services.AddKeyedSingleton<ICommandHandler, PingCommandHandler>(CommandType.Ping);
        //builder.Services.AddRazorPages();
        //builder.Services.AddControllers();
        if (true) // TODO: Check install settings to see if we are using remote or local for handlers.
        {
            /*builder.Services.AddSingleton<IVersionManagementService, VersionManagementService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IUserManagementService, UserManagementService>();
            builder.Services.AddSingleton<IAccountManagementService, AccountManagementService>();
            builder.Services.AddSingleton<ILocationManagementService, LocationManagementService>();
            builder.Services.AddSingleton<IAccessPointManagementService, AccessPointManagementService>();*/
        }
        else
        {
            /*builder.Services.AddSingleton<IVersionManagementService, VersionManagementService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IUserManagementService, UserManagementService>();
            builder.Services.AddSingleton<IAccountManagementService, AccountManagementService>();
            builder.Services.AddSingleton<ILocationManagementService, LocationManagementService>();
            builder.Services.AddSingleton<IAccessPointManagementService, AccessPointManagementService>();*/
        }
        var app = builder.Build();
        //app.UseHttpsRedirection();
        //app.UseAuthorization();
        //app.MapControllers();
        await app.RunAsync();
    }
}