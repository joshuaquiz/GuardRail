using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GuardRail.Authorizers.AlwaysAllowAuthorizer;
using GuardRail.Core;
using GuardRail.Data;
using GuardRail.DeviceProviders;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PCSC;
using Serilog;

namespace GuardRail
{
    internal class Program
    {
        private static async Task Main(string[] _)
        {
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();
            var serviceCollection = new ServiceCollection()
                .AddSingleton<ILogger>(logger);
            serviceCollection.AddSingleton(typeof(ISCardContext), new SCardContext());
            serviceCollection.AddSingleton(typeof(IEventBus), new InMemoryEventBus());
            serviceCollection.AddSingleton(typeof(IAuthorizer), new AlwaysAllowAuthorizer());
            serviceCollection.AddSingleton(typeof(IDeviceProvider), new LoggerDeviceProvider(logger));
            RegisterAllImplementations(serviceCollection, typeof(IDoorFactory));
            RegisterAllImplementations(serviceCollection, typeof(IAccessControlFactory));
            AddDatabaseConfiguration(serviceCollection);
            var serviceProvider = serviceCollection.BuildServiceProvider();
            logger.Debug("Starting application");
            var coordinator = new Coordinator(
                logger,
                serviceProvider.GetRequiredService<GuardRailContext>(),
                serviceProvider.GetRequiredService<IEventBus>(),
                serviceProvider.GetRequiredService<IAuthorizer>(),
                serviceProvider.GetRequiredService<IEnumerable<IDoorFactory>>(),
                serviceProvider.GetRequiredService<IEnumerable<IAccessControlFactory>>());
            await coordinator.StartUp();
            Console.ReadLine();
        }

        private static void RegisterAllImplementations(IServiceCollection serviceCollection, Type type)
        {
            foreach (var t in Assembly
                .GetExecutingAssembly()
                .DefinedTypes
                .Where(x => type.IsAssignableFrom(x) && type != x.AsType()))
            {
                serviceCollection.AddSingleton(type, t.AsType());
            }
        }

        private static void AddDatabaseConfiguration(
            IServiceCollection services) =>
            services
                .AddDbContext<GuardRailContext>(options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()));
    }
}