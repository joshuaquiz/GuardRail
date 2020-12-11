using System.Windows;

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
            StartupHelper.Startup(
                e,
                new MainWindow());
        }
    }
}