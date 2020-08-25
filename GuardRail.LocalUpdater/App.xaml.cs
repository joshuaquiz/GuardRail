using System.Windows;

namespace GuardRail.LocalUpdater
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

        private static void Application_Startup(object sender, StartupEventArgs e) =>
            new MainWindow(e.Args.Length == 0)
                .Show();
    }
}