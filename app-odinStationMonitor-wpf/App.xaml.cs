using app_odinStationMonitor_Messaging;
using CommunityToolkit.Mvvm.Messaging;
using Jendamark.Messaging;
using Jendamark.Messaging.ZRE;
using Jendamark.ODINWorkStationV2.LocalInformationCache.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Configuration;
using System.Data;
using System.Windows;
using TacoStationMonitor.Config;
using TacoStationMonitor.Services;

namespace app_odinStationMonitor_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private ZREMessagingStarter? _zreStarter;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load application configuration
            var builder = new ConfigurationBuilder()
                                             .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                                             .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

            IConfiguration configuration = builder.Build();

            // Get LIC configuration
            var licOptions = new LicOptions();
            configuration.GetSection("LIC").Bind(licOptions);

            // Get the AppSettings section from the configuration
            var appSettings = new AppSettings();
            configuration.GetSection("Settings").Bind(appSettings);

            // Read ODIN configuration from LIC
            var settingsService = new ConsoleSettingsService();
            ConsoleSettingsModel settings = settingsService.GetSettings(licOptions.DatabasePath);
            Jendamark.Messaging.IMessenger messenger = InitializeZRE(settings, appSettings);

            // Configure DI
            _serviceProvider = InitializeServices(configuration, settings, messenger);

            // We'll resolve and show MainWindow here later


        }

        private  ServiceProvider InitializeServices(IConfiguration configuration, ConsoleSettingsModel settings, Jendamark.Messaging.IMessenger messenger)
        {
            var services = new ServiceCollection();

            // Application configuration
            services.AddSingleton(configuration);

            // LIC settings 
            services.AddSingleton(settings);

            // LIC
            services.AddSingleton<IConsoleSettingsService, ConsoleSettingsService>();

            // ZRE
            services.AddSingleton(messenger);

            // DAL
            // services.AddSingleton<IStationRepository, StationRepository>();

            // Messaging
            // services.AddSingleton<IZreService, ZreService>();

            // ViewModels
            // services.AddTransient<MainViewModel>();

            // Views
            // services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }

        private Jendamark.Messaging.IMessenger InitializeZRE(ConsoleSettingsModel settings, AppSettings appSettings)
        {            
            int appIndex = 0;

            var station = settings.Stations.Single(x => x.StationID == appSettings.StationId);

            var subStation = station.SubStations.Single();

            int stationId = station.StationID;
            int subStationIndex = subStation.SubStationIndex;

            var domains = subStation.ZREDomains.ToHashSet();

            string baseName = $"STN{stationId}SUBSTN{subStationIndex}";
            _zreStarter = new ZREMessagingStarter(
                // logger,
                baseName,
                settings.ZRENetworkInterfaceAddress,
                settings.ZREBroadcastPort,
                settings.ZREBroadcastIntervalInSeconds,
                domains,
                appIndex);

            _zreStarter.Start();

            return _zreStarter.Messenger;
        }


        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();

            base.OnExit(e);
        }
    }
}
