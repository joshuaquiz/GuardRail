using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;

namespace GuardRail.LocalClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            VersionTag.Content = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStateChanged(EventArgs e)
        {
            if (WindowState == WindowState.Minimized)
            {
                Hide();
            }

            base.OnStateChanged(e);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
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

        internal void ResetBody()
        {
            foreach (UIElement gridChild in Body.Children)
            {
                gridChild.Visibility = Visibility.Collapsed;
            }
        }

        internal void ActivateSetupScreen()
        {
            SetupControl.Visibility = Visibility.Visible;
            MainGrid.Visibility = Visibility.Collapsed;
            UpdateLayout();
        }

        internal void ActivateHomeScreen()
        {
            ResetBody();
            HomePageUserControl.Visibility = Visibility.Visible;
            UpdateLayout();
        }

        private void HomeMenuItemControl_OnPreviewMouseDown(object sender, MouseButtonEventArgs e) =>
            ActivateHomeScreen();
    }
}