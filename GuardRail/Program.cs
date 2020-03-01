using System;
using System.Linq;
using System.Reflection;
using GuardRail.Definitions;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace GuardRail
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var log = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
            var serviceCollection = new ServiceCollection()
                .AddSingleton<ILogger>(log);
            RegisterAllImplementations(serviceCollection, typeof(IDoor));
            RegisterAllImplementations(serviceCollection, typeof(IAccessControlFactory));
            var serviceProvider = serviceCollection.BuildServiceProvider();

            var logger = serviceProvider.GetService<ILogger>();
            logger.Debug("Starting application");

            // TODO: Add logic.
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
    }
}