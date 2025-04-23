using System;
using System.IO;
using System.Linq;
using System.Reflection;
using GuardRail.Core.Enums;
using GuardRail.Core.Helpers;
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
            where TCommandHandler : class, ICommandHandler
        {
            var propertyValue = typeof(TCommandHandler)
                .GetProperty(
                    nameof(CommandType),
                    BindingFlags.Static | BindingFlags.Public | BindingFlags.GetProperty)
                ?.GetValue(null);
            return serviceCollection
                .AddKeyedSingleton<ICommandHandler, TCommandHandler>(
                    propertyValue is CommandType c
                        ? c
                        : throw new NotImplementedException($"The implementation for {typeof(TCommandHandler).Name} does not include a property called {nameof(CommandType)} or type {nameof(CommandType)}."));
        }

        public static IServiceCollection AddGuardRailSupportedHardware(
            this IServiceCollection serviceCollection)
        {
            // Get all currently loaded assemblies.
            var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(a => a.Location).ToArray();

            // Find and load all referenced assemblies that aren't already loaded.
            var referencedPaths = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase));

            // Load the assemblies.
            foreach (var path in toLoad)
            {
                try
                {
                    var assemblyName = AssemblyName.GetAssemblyName(path);
                    var assembly = Assembly.Load(assemblyName);
                    loadedAssemblies.Add(assembly);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading assembly from {path}: {ex.Message}");
                }
            }

            // Filter out system and Microsoft assemblies.
            var assemblies = loadedAssemblies
                .Where(x =>
                    !x.FullName.IsNullOrWhiteSpace()
                    && !x.FullName.StartsWith("Microsoft")
                    && !x.FullName.StartsWith("System"));

            // Register all ISupportedHardware implementations.
            foreach (var assembly in assemblies)
            {
                try
                {
                    var supportedHardwareInterface = typeof(ISupportedHardware);
                    var types = assembly
                        .GetTypes()
                        .Where(
                            x =>
                                x is { IsInterface: false, IsAbstract: false }
                                && supportedHardwareInterface
                                    .IsAssignableFrom(
                                        x));
                    foreach (var hardwareType in types)
                    {
                        var setupMethod = hardwareType
                                              .GetMethod(
                                                  nameof(ISupportedHardware.Setup))
                                          ?? throw new NotImplementedException($"The type {hardwareType.Name} does not implement {nameof(ISupportedHardware.Setup)}");
                        setupMethod
                            .Invoke(
                                hardwareType,
                                [
                                    serviceCollection
                                ]);
                    }
                }
                catch (ReflectionTypeLoadException ex)
                {
                    // Handle cases where an assembly's types cannot be loaded.
                    foreach (var loaderException in ex.LoaderExceptions)
                    {
                        Console.WriteLine($"Error loading types from assembly {assembly.FullName}: {loaderException?.Message}");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error processing assembly {assembly.FullName}: {ex.Message}");
                }
            }

            return serviceCollection;
        }
    }
}
