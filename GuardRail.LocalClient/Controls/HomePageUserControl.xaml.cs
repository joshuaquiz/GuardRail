using System;
using System.Collections.Generic;

namespace GuardRail.LocalClient.Controls
{
    /// <summary>
    /// Interaction logic for HomePageUserControl.xaml
    /// </summary>
    public partial class HomePageUserControl
    {
        public List<LogItem> Logs { get; set; } = new List<LogItem>();

        /// <summary>
        /// Interaction logic for HomePageUserControl.xaml
        /// </summary>
        public HomePageUserControl()
        {
            Logs.Add(
                new LogItem
                {
                    TimeStamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                    Log = "Startup!"
                });
            InitializeComponent();
        }

        public class LogItem
        {
            public string TimeStamp { get; set; }
            
            public string Log { get; set; }
        }
    }
}