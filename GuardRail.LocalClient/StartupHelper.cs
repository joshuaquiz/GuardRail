using GuardRail.Core.CommandLine;

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
        public static void Startup(CommandLineArguments commandLineArguments, MainWindow mainWindow)
        {
            if (commandLineArguments.ContainsKey(CommandLineArgumentType.FreshInstall))
            {
                SetupDatabase();
            }
            
            if (commandLineArguments.ContainsKey(CommandLineArgumentType.ShouldShowSetup))
            {
                mainWindow.ActivateSetupScreen();
            }
        }

        private static void SetupDatabase()
        {
            
        }
    }
}