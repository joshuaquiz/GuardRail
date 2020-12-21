using System.Threading.Tasks;
using GuardRail.Core.CommandLine;
using GuardRail.LocalClient.Data.Local;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient
{
    /// <summary>
    /// Helps with any startup actions or setup that needs to happen.
    /// </summary>
    public class StartupHelper
    {
        /// <summary>
        /// Helps with any startup actions or setup that needs to happen.
        /// </summary>
        /// <param name="commandLineArguments">Command line arguments passed to the main app.</param>
        /// <param name="mainWindow">The application's main window.</param>
        public static async Task StartupAsync(
            CommandLineArguments commandLineArguments,
            MainWindow mainWindow)
        {
            if (commandLineArguments.ContainsKey(CommandLineArgumentType.FreshInstall)
                || commandLineArguments.ContainsKey(CommandLineArgumentType.ShouldShowSetup))
            {
                await SetupDatabaseAsync();
                mainWindow.ActivateSetupScreen();
                return;
            }

            mainWindow.ActivateHomeScreen();
        }

        private static async Task SetupDatabaseAsync()
        {
            using var serviceScope = App.ServiceProvider.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var context = serviceScope.ServiceProvider.GetRequiredService<GuardRailContext>();
            await context.Database.MigrateAsync();
        }
    }
}