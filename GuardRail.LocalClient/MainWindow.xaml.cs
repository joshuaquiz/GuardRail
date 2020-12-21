using System;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using GuardRail.LocalClient.Interfaces;
using GuardRail.LocalClient.Setup;

namespace GuardRail.LocalClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly IGuardRailApiClient _guardRailApiClient;
        private bool _loading;

        /// <summary>
        /// Shows the loading icon.
        /// </summary>
        public bool Loading
        {
            get => _loading;
            set
            {
                _loading = value;
                Cursor = Cursors.Wait;
            }
        }

        /// <summary>
        /// Interaction logic for MainWindow.xaml
        /// </summary>
        public MainWindow(IGuardRailApiClient guardRailApiClient)
        {
            _guardRailApiClient = guardRailApiClient;
            InitializeComponent();
            VersionTag.Content = $"Version: {Assembly.GetExecutingAssembly().GetName().Version}";
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
            foreach (UIElement gridChild in Grid.Children)
            {
                gridChild.Visibility = Visibility.Collapsed;
            }

            var setupUserControl = new SetupUserControl(_guardRailApiClient);
            setupUserControl.SetValue(System.Windows.Controls.Grid.ColumnProperty, 0);
            setupUserControl.SetValue(System.Windows.Controls.Grid.RowProperty, 0);
            setupUserControl.SetValue(System.Windows.Controls.Grid.ColumnSpanProperty, 2);
            setupUserControl.SetValue(System.Windows.Controls.Grid.RowSpanProperty, 3);
            Grid.Children.Add(setupUserControl);
        }
        
        private void HomeMenuItemControl_OnPreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            foreach (UIElement item in Body.Children)
            {
                item.Visibility = Visibility.Collapsed;
            }

            HomePageUserControl.Visibility = Visibility.Visible;
        }
    }
}