using System;
using System.Linq;
using System.Reflection;
using GuardRail.Database.Main;
using GuardRail.Hardware.Common;
using GuardRail.Logic.Commands.Implementations;
using GuardRail.Logic.Commands.Interfaces;
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

        public static IServiceCollection AddGuardRailCommandHandlers(
            this IServiceCollection serviceCollection)
        {
            serviceCollection.AddCommandHandler<PingTypedCommandHandler>();
            serviceCollection.AddCommandHandler<GetAvailableAccessPointsTypedCommandHandler>();
            return serviceCollection;
        }

        public static IServiceCollection AddCommandHandler<TCommandHandler>(
            this IServiceCollection serviceCollection)
            where TCommandHandler : class, ICommandHandler =>
            serviceCollection
                .AddKeyedSingleton<ICommandHandler, TCommandHandler>(
                    typeof(TCommandHandler)
                            .GetProperty(
                                nameof(ICommandHandler.CommandType),
                                BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty)
                            !.GetValue(null)
                        as string);

        public static IServiceCollection AddGuardRailSupportedHardware(
            this IServiceCollection serviceCollection)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                try
                {
                    var types = assembly.GetTypes()
                        .Where(
                            x =>
                                x is { IsInterface: false, IsAbstract: false }
                                && typeof(ISupportedHardware)
                                    .IsAssignableFrom(
                                        x));
                    foreach (var hardwareType in types)
                    {
                        serviceCollection.AddSingleton(hardwareType);
                        serviceCollection.AddSingleton(typeof(ISupportedHardware), hardwareType);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle cases where an assembly's types cannot be loaded
                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        Console.WriteLine($"Error loading types from assembly {assembly.FullName}: {loaderException?.Message}");
                    }
                }
            }

            return serviceCollection;
        }
    }
}
