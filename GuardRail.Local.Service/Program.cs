using System.Threading.Tasks;
using GuardRail.Logic.Implementations;
using GuardRail.Logic.Interfaces;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace GuardRail.Local.Service;

public static class Program
{
    public static async Task Main(
        string[] args)
    {
        var builder = WebApplication.CreateBuilder(
            args);
        builder.Services.AddWindowsService();
        builder.Services.AddHostedService<AutoUpdateCheckerWorker>();
        builder.Services.AddHostedService<DataSyncWorker>();
        builder.Services.AddHostedService<UdpPingListenerWorker>();
        builder.Services.AddRazorPages();
        builder.Services.AddControllers();
        if (true) // TODO: Check install settings to see if we are using remote or local for handlers.
        {
            builder.Services.AddSingleton<IVersionManagementService, VersionManagementService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IUserManagementService, UserManagementService>();
            builder.Services.AddSingleton<IAccountManagementService, AccountManagementService>();
            builder.Services.AddSingleton<ILocationManagementService, LocationManagementService>();
            builder.Services.AddSingleton<IAccessPointManagementService, AccessPointManagementService>();
        }
        else
        {
            builder.Services.AddSingleton<IVersionManagementService, VersionManagementService>();
            builder.Services.AddSingleton<IEmailService, EmailService>();
            builder.Services.AddSingleton<IUserManagementService, UserManagementService>();
            builder.Services.AddSingleton<IAccountManagementService, AccountManagementService>();
            builder.Services.AddSingleton<ILocationManagementService, LocationManagementService>();
            builder.Services.AddSingleton<IAccessPointManagementService, AccessPointManagementService>();
        }
        var app = builder.Build();
        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        await app.RunAsync();
    }
}