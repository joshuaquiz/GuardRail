using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GuardRail.Core.CommandLine;
using GuardRail.Core.Data;
using GuardRail.Core.Helpers;
using GuardRail.LocalClient.Data.Local;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient;

/// <summary>
/// Helps with any startup actions or setup that needs to happen.
/// </summary>
public static class StartupHelper
{
    /// <summary>
    /// Helps with any startup actions or setup that needs to happen.
    /// </summary>
    /// <param name="commandLineArguments">Command line arguments passed to the main app.</param>
    /// <param name="mainWindow">The application's main window.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    public static async Task StartupAsync(
        CommandLineArguments commandLineArguments,
        MainWindow mainWindow,
        CancellationToken cancellationToken)
    {
        if (commandLineArguments.ContainsKey(CommandLineArgumentType.FreshInstall)
            || commandLineArguments.ContainsKey(CommandLineArgumentType.ShouldShowSetup))
        {
            GuardRailBackgroundWorker.GlobalStop = true;
            await SetupDatabaseAsync(cancellationToken);
            mainWindow.ActivateSetupScreen();
            return;
        }

        GuardRailBackgroundWorker.GlobalStop = false;
        mainWindow.ActivateHomeScreen();
    }

    private static async Task SetupDatabaseAsync(CancellationToken cancellationToken)
    {
        using var serviceScope = App.Host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
        var context = serviceScope.ServiceProvider.GetRequiredService<GuardRailContext>();
        var pendingMigrations = await context.Database.GetPendingMigrationsAsync(cancellationToken);
        if (pendingMigrations.Any())
        {
            await context.Database.MigrateAsync(cancellationToken);
        }
    }
}