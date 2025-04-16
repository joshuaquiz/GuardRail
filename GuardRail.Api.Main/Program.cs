using GuardRail.Logic.Helpers;

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
        builder.Services.AddGuardRailLocalServices();
        var app = builder.Build();
        app.UseGuardRailApi();
        await app.RunAsync();
    }
}