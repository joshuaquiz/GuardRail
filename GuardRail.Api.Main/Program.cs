using GuardRail.Database.Main;
using GuardRail.Logic.Implementations;
using GuardRail.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace GuardRail.Api.Main;

public static class Program
{
    public static async Task Main(
        string[] args)
    {
        var builder = WebApplication
            .CreateBuilder(
                args);

        // Add services to the container.
        builder.Services.AddGuardRailApi();

        builder.Services.AddDbContextFactory<GuardRailDbContext>(
            x =>
                x.UseInMemoryDatabase(
                    Guid.NewGuid().ToString()));

        builder.Services.AddSingleton<IVersionManagementService, VersionManagementService>();
        builder.Services.AddSingleton<ILocationCommunicationService, LocationCommunicationService>();
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddSingleton<IUserManagementService, UserManagementService>();
        builder.Services.AddSingleton<IAccountManagementService, AccountManagementService>();
        builder.Services.AddSingleton<ILocationManagementService, LocationManagementService>();
        builder.Services.AddSingleton<IAccessPointManagementService, AccessPointManagementService>();
        builder.Services.AddSingleton<ICommandManagementService, CommandManagementService>();

        var app = builder.Build();
        app.UseGuardRailApi();
        await app.RunAsync();
    }
}