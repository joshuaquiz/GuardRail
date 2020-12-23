namespace GuardRail.LocalClient.Controls.Pages.Users
{
    /// <summary>
    /// Interaction logic for UsersUserControl.xaml
    /// </summary>
    public partial class UsersPageUserControl
    {
        /// <summary>
        /// The viewModel.
        /// </summary>
        public UsersPageViewModel ViewModel;

        /// <summary>
        /// Interaction logic for UsersUserControl.xaml
        /// </summary>
        public UsersPageUserControl()
        {
            InitializeComponent();
            ViewModel = (UsersPageViewModel)DataContext;
        }
    }
}