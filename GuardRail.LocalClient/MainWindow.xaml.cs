using System;
using System.ComponentModel;
using System.Windows;

namespace GuardRail.LocalClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            e.Cancel = true;
            Hide();
            base.OnClosing(e);
        }

        private void TaskBarIcon_TrayMouseDoubleClick(object sender, RoutedEventArgs e) =>
            Show();

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e) =>
            Application.Current.Shutdown();

        internal void ActivateSetupScreen()
        {
            SetupUserControl.Visibility = Visibility.Visible;
        }
    }
}