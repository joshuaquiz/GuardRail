using GuardRail.Database.Main;
using GuardRail.Logic.Implementations;
using GuardRail.Logic.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.Logic.Helpers
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGuardRailLocalServices(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddDbContextFactory<GuardRailDbContext>(
                x =>
                    x.UseInMemoryDatabase(
                        "GuardRail"));
            serviceCollection.AddSingleton<IVersionManagementService, VersionManagementService>();
            serviceCollection.AddSingleton<ILocationCommunicationService, LocationCommunicationService>();
            serviceCollection.AddSingleton<IEmailService, EmailService>();
            serviceCollection.AddSingleton<IUserManagementService, UserManagementService>();
            serviceCollection.AddSingleton<IAccountManagementService, AccountManagementService>();
            serviceCollection.AddSingleton<ILocationManagementService, LocationManagementService>();
            serviceCollection.AddSingleton<IAccessPointManagementService, AccessPointManagementService>();
            serviceCollection.AddSingleton<ICommandManagementService, CommandManagementService>();
            return serviceCollection;
        }
    }
}
