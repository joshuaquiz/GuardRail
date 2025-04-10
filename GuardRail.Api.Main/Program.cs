using GuardRail.Logic.Implementations;
using GuardRail.Logic.Interfaces;

namespace GuardRail.Api.Main;

public static class Program
{
    public static async Task Main(
        string[] args)
    {
        var builder = WebApplication.CreateBuilder(
            args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
        //builder.Services.AddOpenApi();

        // TODO: build auth validation attrs.
        builder.Services.AddSingleton<IVersionManagementService, VersionManagementService>();
        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddSingleton<IUserManagementService, UserManagementService>();
        builder.Services.AddSingleton<IAccountManagementService, AccountManagementService>();
        builder.Services.AddSingleton<ILocationManagementService, LocationManagementService>();
        builder.Services.AddSingleton<IAccessPointManagementService, AccessPointManagementService>();
        builder.Services.AddSingleton<ICommandManagementService, CommandManagementService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            //app.MapOpenApi();
        }

        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        await app.RunAsync();
    }
}