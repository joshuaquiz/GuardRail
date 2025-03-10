using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GuardRail.Database.Main.InternalConfiguration;

public class GuardRailDbContextFactory
    : IDesignTimeDbContextFactory<GuardRailDbContext>
{
    public GuardRailDbContextFactory()
    {

    }

    private readonly IConfiguration _configuration;

    public GuardRailDbContextFactory(
        IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public GuardRailDbContext CreateDbContext(
        string[] args)
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .AddJsonFile(
                "appSettings.json")
            .Build();
        var optionsBuilder = new DbContextOptionsBuilder<GuardRailDbContext>();
        optionsBuilder.UseInMemoryDatabase(
            configuration["DatabaseName"]!);
        return new GuardRailDbContext(
            optionsBuilder.Options);
    }
}