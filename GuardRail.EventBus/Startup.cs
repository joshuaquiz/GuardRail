using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;

namespace GuardRail.EventBus;

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
        services.AddSignalR();
        services.AddEntityFrameworkSqlite()
            .AddDbContext<GuardRailEventDbContext>();
        services.AddSingleton<GuardRailHub, GuardRailHub>();
        services.AddAuthentication("BasicAuthentication")
            .AddScheme<AuthenticationSchemeOptions, BasicAuthenticationHandler>("BasicAuthentication", null);
    }

    private static void MigrateDatabases(IApplicationBuilder app)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        var dmsContext = serviceScope.ServiceProvider.GetRequiredService<GuardRailEventDbContext>();
        MigrateDb(dmsContext);
    }

    private static void MigrateDb<T>(T context)
        where T : DbContext
    {
        var databaseFacade = context.Database;
        if (databaseFacade.GetMigrations().Any()
            && databaseFacade.GetPendingMigrations().Any())
        {
            databaseFacade.Migrate();
        }
    }

    public void Configure(
        IApplicationBuilder app,
        IWebHostEnvironment env)
    {
        MigrateDatabases(app);
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

/// <summary>
/// An event and its data
/// </summary>
internal sealed class EventData
{
    [Key]
    public long Id { get; set; }

    [Required]
    [MaxLength(256)]
    internal string Type { get; set; }

    [Required]
    internal string Data { get; set; }

    [Required]
    internal DateTime CreatedOn { get; set; }

    public static void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<EventData>().ToTable("EventData");
        builder.Entity<EventData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Type);
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}

/// <summary>
/// Tracks where a client is in the list of events
/// </summary>
internal sealed class ClientLocationData
{
    [Key]
    public long Id { get; set; }

    [Required]
    internal Guid ClientId { get; set; }

    [Required]
    [MaxLength(256)]
    internal string Type { get; set; }

    [Required]
    internal long LastLocationReturned { get; set; }

    [Required]
    internal DateTime LastPolledOn { get; set; }

    [Required]
    internal DateTime CreatedOn { get; set; }

    public static void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<ClientLocationData>().ToTable("ClientLocationData");
        builder.Entity<ClientLocationData>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => new { e.Type, e.ClientId });
            entity.Property(e => e.CreatedOn).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });
    }
}

internal sealed class GuardRailEventDbContext : DbContext
{
    internal DbSet<EventData> Events { get; set; }

    internal DbSet<ClientLocationData> ClientLocations { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Filename=GuardRailEventData.db");
    }
}
public interface IGuardRailEvent
{
    string Name { get; }

    string Data { get; }
}

public interface IEventPublisher
{
    Task PublishEvent(IGuardRailEvent e);
}

public interface IEventHub
{
    void Startup(ICollection<Type> events);

    void Shutdown();
}

public sealed class TestEvent : IGuardRailEvent
{
    public string Name => "Test";

    public string Data { get; set; }
}

public sealed class EventHub : IEventHub
{
    private readonly IDictionary<Type, ObservableCollection<IGuardRailEvent>> _observables = new Dictionary<Type, ObservableCollection<IGuardRailEvent>>();
    private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

    public void Startup(ICollection<Type> events)
    {
        if (events.Any(x => !x.IsAssignableFrom(typeof(IGuardRailEvent))))
        {
            throw new ArgumentException("Some values are not of type IGuardRailEvent", nameof(events));
        }

        var taskFactory = new TaskFactory();
        var eventTasks = events
            .Select(x =>
                taskFactory
                    .StartNew(
                        async () =>
                        {
                            _observables[x] = new ObservableCollection<IGuardRailEvent>();
                            await using var pipeServer = new NamedPipeServerStream(
                                x.Name + "-in",
                                PipeDirection.In,
                                int.MaxValue,
                                PipeTransmissionMode.Message,
                                PipeOptions.Asynchronous)
                            {
                                ReadMode = PipeTransmissionMode.Message
                            };
                            await pipeServer.WaitForConnectionAsync(_cancellationTokenSource.Token);
                            var reader = new StreamReader(pipeServer);
                            while (!reader.EndOfStream)
                            {
                                var line = await reader.ReadLineAsync();
                                _observables[x].Add(
                                    JsonConvert.DeserializeObject<IGuardRailEvent>(
                                        line));
                            }
                        },
                        _cancellationTokenSource.Token,
                        TaskCreationOptions.LongRunning,
                        TaskScheduler.Current));
        foreach (var eventTask in eventTasks)
        {
            eventTask.Start();
        }
    }

    public void Shutdown() =>
        _cancellationTokenSource.Cancel();
}