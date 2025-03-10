using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Database.Main.InternalConfiguration;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection ConfigureService(
        this IServiceCollection service,
        IConfiguration configuration)
    {
        configuration = new ConfigurationBuilder()
            .AddJsonFile(
                "appSettings.json")
            .Build();
        service.AddDbContext<GuardRailDbContext>(
            options =>
                options.UseInMemoryDatabase(
                    configuration["DatabaseName"]!));
        return service;
    }
}