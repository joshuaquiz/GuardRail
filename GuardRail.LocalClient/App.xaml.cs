using System;
using System.IO;
using System.Threading;
using System.Windows;
using GuardRail.Core.CommandLine;
using GuardRail.LocalClient.Data;
using GuardRail.LocalClient.Data.Interfaces;
using GuardRail.LocalClient.Data.Local;
using GuardRail.LocalClient.Data.Models;
using GuardRail.LocalClient.Data.Remote;
using GuardRail.LocalClient.Implementations;
using GuardRail.LocalClient.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GuardRail.LocalClient
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Service provider for the application.
        /// </summary>
        public static IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Configuration for the application.
        /// </summary>
        public static IConfiguration Configuration { get; private set; }

        /// <summary>
        /// The CancellationTokenSource for the application.
        /// </summary>
        public static CancellationTokenSource CancellationTokenSource { get; private set; }

        /// <summary>
        /// The account for this installation. 
        /// </summary>
        public static Account Account { get; private set; }

        /// <summary>
        /// Initial app startup configuration.
        /// </summary>
        public App()
        {
            Startup += Application_Startup;
            Exit += Application_Exit;
        }

        private async void Application_Startup(object sender, StartupEventArgs e)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false, true)
                .AddJsonFile("appsettings.Development.json", false, true);
            Configuration = builder.Build();
            Account = new Account
            {
                Id = 1,
                Guid = Guid.NewGuid(),
                Location = "Here",
                Name = "Test"
            };
            CancellationTokenSource = new CancellationTokenSource();
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            ServiceProvider = serviceCollection.BuildServiceProvider();
            var dataStore = ServiceProvider.GetRequiredService<IDataStore>();
            await dataStore.StartAsync(CancellationTokenSource.Token);
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            await StartupHelper.StartupAsync(
                CommandLineArguments.Create(e.Args),
                mainWindow,
                CancellationTokenSource.Token);
            mainWindow.Show();
        }
        
        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<MainWindow>();
            services.AddSingleton<IGuardRailFileProvider>(new GuardRailFileProvider(Directory.GetCurrentDirectory()));
            services.AddSingleton<IGuardRailApiClient, GuardRailApiClient>();
            services.AddDbContext<GuardRailContext>(x => x.UseSqlite("Data Source=LocalDataStore.db"));
            services.AddSingleton<IDataSink, EntityFrameWorkDataSink>();
            services.AddSingleton<IDataSink, RemoteDataSink>();
            services.AddSingleton<IDataStore, DataStore>();
            services.AddHostedService<DataStore>();
        }

        private static void Application_Exit(object sender, ExitEventArgs e)
        {
            CancellationTokenSource.Cancel();
            var disposables = ServiceProvider.GetServices<IDisposable>();
            foreach (var disposable in disposables)
            {
                disposable.Dispose();
            }
        }
    }
}