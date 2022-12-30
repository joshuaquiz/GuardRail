using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Threading;
using GuardRail.Core.Data.Models;
using GuardRail.Core.Helpers;
using GuardRail.LocalClient.Data.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient.Controls.Pages.Users;

/// <summary>
/// View model for the users page user control.
/// </summary>
public sealed class UsersPageViewModel : IDisposable
{
    private readonly GuardRailBackgroundWorker _guardRailBackgroundWorker;

    /// <summary>
    /// For viewing users in the application.
    /// </summary>
    public ObservableCollection<User> Users { get; set; } = new();

    /// <summary>
    /// The user being edited.
    /// </summary>
    public User EditingUser { get; set; }

    /// <summary>
    /// View model for the users page user control.
    /// </summary>
    public UsersPageViewModel()
    {
        if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
        {
            Users = new ObservableCollection<User>(
                new List<User>
                {
                    new()
                    {
                        FirstName = "Joshua1",
                        LastName = "Galloway1",
                        Username = "username1",
                        Password = "password1".GetBytes(),
                        Email = "asdf@asdf1.com",
                        Phone = "3213213214"
                    },
                    new()
                    {
                        FirstName = "Joshua2",
                        LastName = "Galloway2",
                        Username = "username2",
                        Password = "password2".GetBytes(),
                        Email = "asdf@asdf2.com",
                        Phone = "3213213214"
                    },
                    new()
                    {
                        FirstName = "Joshua3",
                        LastName = "Galloway3",
                        Username = "username3",
                        Password = "password3".GetBytes(),
                        Email = "asdf@asdf3.com",
                        Phone = "3213213214"
                    }
                });
            EditingUser = Users.First();
        }
        else
        {

            _guardRailBackgroundWorker = GuardRailBackgroundWorker.Create(
                "Users display sync",
                TimeSpan.FromSeconds(1),
                async ct =>
                {
                    var dataStore = App.Host.Services.GetRequiredService<IDataStore>();
                    foreach (var user in await dataStore.GetData<User>(x => x.AccountGuid == App.Account.Guid, ct))
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
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _guardRailBackgroundWorker?.Dispose();
    }
}