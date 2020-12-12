using System.Windows;
using GuardRail.Core.CommandLine;

namespace GuardRail.LocalClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Initial app startup configuration.
        /// </summary>
        public App()
        {
            Startup += Application_Startup;
        }

        private static void Application_Startup(object sender, StartupEventArgs e)
        {
            var mainWindow = new MainWindow();
            StartupHelper.Startup(
                CommandLineArguments.Create(e.Args),
                mainWindow);
            mainWindow.Show();
        }
    }
}