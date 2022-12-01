using System;
using GuardRail.Api.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;

namespace GuardRail.Api;

public sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        Serilog.Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .Enrich.WithExceptionDetails()
            .CreateLogger();
        services.AddLogging();
        services.AddSingleton(Serilog.Log.Logger);
        services.AddDatabaseDeveloperPageExceptionFilter();
        services.AddControllersWithViews();
        services.AddRazorPages();
        services.AddHealthChecks();
        services.AddSignalR();
        services.AddCors();
        services.AddDbContext<GuardRailContext>(
            options => options.UseInMemoryDatabase(
                Guid.NewGuid().ToString()),
                ServiceLifetime.Singleton,
                ServiceLifetime.Singleton);
        services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
        services.AddSingleton<GuardRailHub>();
        Serilog.Log.Logger.Debug("Starting application");
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseMigrationsEndPoint();
        }
        else
        {
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();
        app.UseRouting();
        app.UseCors();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
            endpoints.MapRazorPages();
            endpoints.MapFallbackToFile("index.html");
            endpoints.MapHealthChecks("/healthz");
            endpoints.MapHub<GuardRailHub>("/guardRailHub");
        });
    }
}