using System;
using System.Collections.Generic;
using System.Linq;
using GuardRail.Core.Data;
using GuardRail.Core.Data.Models;
using GuardRail.Core.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Serilog.Exceptions;
using Microsoft.AspNetCore.Routing;

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
        services.AddRouting();
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

        var db = app.ApplicationServices.GetRequiredService<GuardRailContext>();
        var account = new Account
        {
            Guid = Guid.NewGuid(),
            Location = "lolz",
            Name = "lolz"
        };
        ApplicationGlobals.Account = account;
        db.Accounts.Add(account);
        db.Users.Add(
            new User
            {
                AccountGuid = account.Guid,
                FirstName = "Joshua",
                LastName = "Galloway",
                Email = "joshuaquiz@gmail.com",
                Phone = "3177603538",
                Password = "asdf".GetBytes(),
                Username = "asdf"
            });
        db.SaveChanges();

        var endpoints = app.ApplicationServices.GetRequiredService<IEnumerable<EndpointDataSource>>()
            .SelectMany(es => es.Endpoints)
            .OfType<RouteEndpoint>()
            .Where(x => !(x.RoutePattern.RawText?.Equals("/health") ?? false))
            .Select(x => $"{x.Metadata.OfType<HttpMethodMetadata>().FirstOrDefault()?.HttpMethods[0] ?? "Unknown Verb"} /{x.RoutePattern.RawText?.TrimStart('/')}")
            .Distinct()
            .OrderBy(x => x)
            .ToList();
        Console.WriteLine(endpoints.ToJson());
    }
}