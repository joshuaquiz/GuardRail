using System;
using System.Linq;
using System.Reflection;
using GuardRail.Api.Authorizers;
using GuardRail.Api.Data;
using GuardRail.Api.DeviceProviders;
using GuardRail.Api.Doors;
using GuardRail.Core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PCSC;
using Serilog;
using ILogger = Serilog.ILogger;

namespace GuardRail.Api
{
    public sealed class Startup
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
            services.AddSignalR();
            services.AddDbContext<GuardRailContext>(
                options => options.UseInMemoryDatabase(Guid.NewGuid().ToString()),
                ServiceLifetime.Transient,
                ServiceLifetime.Singleton);
            services.AddSingleton<ILogger>(logger);
            services.AddSingleton<ISCardContext, SCardContext>();
            services.AddSingleton<IEventBus, DefaultEventBus>();
            services.AddSingleton<IAuthorizer, DefaultAuthorizer>();
            services.AddSingleton<IDeviceProvider, DeviceProvider>();
            services.AddSingleton<IDoorResolver, DoorResolver>();
            services.AddAuthentication("BasicAuthentication")
                .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
            services.AddSingleton<GuardRailHub, GuardRailHub>();
            services.AddSingleton<GuardRailLogger, GuardRailLogger>();
            RegisterAllImplementations(services, typeof(IDoorFactory));
            RegisterAllImplementations(services, typeof(IAccessControlFactory));
            services.AddHostedService<CoordinatorService>();
            logger.Debug("Starting application");
        }

        private static void RegisterAllImplementations(
            IServiceCollection serviceCollection,
            Type type)
        {
            foreach (var t in Assembly
                .GetExecutingAssembly()
                .DefinedTypes
                .Where(x => type.IsAssignableFrom(x) && type != x.AsType()))
            {
                serviceCollection.AddSingleton(type, t.AsType());
            }
        }

        public void Configure(
            IApplicationBuilder app,
            IWebHostEnvironment env)
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
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHealthChecks("/healthz");
                endpoints.MapHub<GuardRailHub>("/guardRailHub");
            });
        }

        public void ConfigureHealthChecks(IServiceCollection services)
        {
            services.AddHealthChecks();
        }
    }
}