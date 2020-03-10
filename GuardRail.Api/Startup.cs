using System;
using System.Linq;
using System.Reflection;
using GuardRail.Api.Authorizers.AlwaysAllowAuthorizer;
using GuardRail.Api.Data;
using GuardRail.Api.DeviceProviders;
using GuardRail.Core;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PCSC;
using Serilog;

namespace GuardRail.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            ConfigureHealthChecks(services);
            var logger = new LoggerConfiguration()
                .WriteTo.Console()
                .MinimumLevel.Verbose()
                .CreateLogger();
            services.AddSingleton<ILogger>(logger);
            services.AddSingleton(typeof(ISCardContext), new SCardContext());
            services.AddSingleton(typeof(IEventBus), new InMemoryEventBus());
            services.AddSingleton(typeof(IAuthorizer), new AlwaysAllowAuthorizer());
            services.AddSingleton(typeof(IDeviceProvider), new LoggerDeviceProvider(logger));
            RegisterAllImplementations(services, typeof(IDoorFactory));
            RegisterAllImplementations(services, typeof(IAccessControlFactory));
            AddDatabaseConfiguration(services);
            services.AddHostedService<CoordinatorService>();
            logger.Debug("Starting application");
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

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");
            });
        }

        public void ConfigureHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks();
        }
    }
}