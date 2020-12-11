using System.Windows;

namespace GuardRail.LocalClient
{
    /// <summary>
    /// Helps with any startup actions or setup that needs to happen.
    /// </summary>
    public class StartupHelper
    {
        public static void Startup(StartupEventArgs startupEventArgs, MainWindow mainWindow)
        {
            if (startupEventArgs.Args > 0)
            {

            }

            mainWindow.Show();
        }
    }
}