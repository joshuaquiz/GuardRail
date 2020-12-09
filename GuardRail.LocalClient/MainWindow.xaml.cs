﻿using System;
using System.ComponentModel;

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
            if (WindowState == System.Windows.WindowState.Minimized)
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

        private void TaskBarIcon_TrayMouseDoubleClick(object sender, System.Windows.RoutedEventArgs e) =>
            Show();

        private void CloseMenuItem_Click(object sender, System.Windows.RoutedEventArgs e) =>
            System.Windows.Application.Current.Shutdown();
    }
}