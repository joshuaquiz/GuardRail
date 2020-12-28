using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using GuardRail.Core.Helpers;
using GuardRail.LocalClient.Data.Interfaces;
using GuardRail.LocalClient.Data.Models;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient.Controls.Pages.Users
{
    /// <summary>
    /// View model for the users page user control.
    /// </summary>
    public sealed class UsersPageViewModel : IDisposable
    {
        private readonly GuardRailBackgroundWorker _guardRailBackgroundWorker;

        /// <summary>
        /// For viewing users in the application.
        /// </summary>
        public ObservableCollection<User> Users { get; set; } = new ObservableCollection<User>();

        /// <summary>
        /// View model for the users page user control.
        /// </summary>
        public UsersPageViewModel()
        {
            _guardRailBackgroundWorker = GuardRailBackgroundWorker.Create(
                "Users display sync",
                TimeSpan.FromSeconds(1),
                async ct =>
                {
                    var dataStore = App.ServiceProvider.GetRequiredService<IDataStore>();
                    foreach (var user in await dataStore.GetData<User>(x => x.Account == App.Account, ct))
                    {
                        if (!Users.Contains(user))
                        {
                            await Application.Current.Dispatcher.InvokeAsync(
                                () => Users.Add(user),
                                DispatcherPriority.DataBind);
                        }
                    }
                },
                App.CancellationTokenSource.Token);
            _guardRailBackgroundWorker.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _guardRailBackgroundWorker?.Dispose();
        }
    }
}