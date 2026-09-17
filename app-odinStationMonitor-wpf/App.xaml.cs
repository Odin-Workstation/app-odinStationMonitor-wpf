using app_odinStationMonitor_Messaging;
using CommunityToolkit.Mvvm.Messaging;
using Jendamark.Messaging;
using Jendamark.Messaging.ZRE;
using Jendamark.ODINWorkStationV2.LocalInformationCache.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using System.Configuration;
using System.Data;
using System.Windows;
using TacoStationMonitor.Config;
using TacoStationMonitor.Service;
using TacoStationMonitor.Services;
using TacoStationMonitor.ViewModels;
using TacoStationMonitor.Views;

namespace app_odinStationMonitor_wpf
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private ServiceProvider? _serviceProvider;
        private ZREMessagingStarter? _zreStarter;
        private Serilog.ILogger? _logger;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            _logger = InitializeLogger(configuration);

            try
            {
                _logger.Information("Starting TACO Station Monitor");

                var licOptions = new LicOptions();
                configuration.GetSection("LIC").Bind(licOptions);

                AppSettings? appSettings = new AppSettings();
                configuration.GetSection("Settings").Bind(appSettings);

                var settingsService = new ConsoleSettingsService();

                ConsoleSettingsModel settings = settingsService.GetSettings(licOptions.DatabasePath);

                Jendamark.Messaging.IMessenger messenger = InitializeZRE(settings, appSettings, _logger);

                _serviceProvider = InitializeServices(appSettings, configuration, settings, messenger, _logger);

                var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
                mainWindow.Show();

            }
            catch (Exception ex)
            {
                _logger.Fatal(ex,
                    "TACO Station Monitor failed during startup");

                Shutdown(-1);
            }
        }

        private  ServiceProvider InitializeServices(AppSettings appSettings, IConfiguration configuration, ConsoleSettingsModel settings,
                                                    Jendamark.Messaging.IMessenger messenger, Serilog.ILogger logger)
        {
            var services = new ServiceCollection();

            // Application configuration
            services.AddSingleton(configuration);

            services.AddSingleton(appSettings);

            // LIC settings 
            services.AddSingleton(settings);

            // LIC
            services.AddSingleton<IConsoleSettingsService, ConsoleSettingsService>();

            // ZRE
            services.AddSingleton(messenger);

            // Logging
            services.AddLogging(builder =>
            {
                builder.ClearProviders();
                builder.AddSerilog(logger);
            });

            // Data Service
            services.AddTransient<IDashboardService, DashboardService>();           

            // ViewModels
            services.AddTransient<DashboardViewModel>();

            // Views
            services.AddTransient<MainWindow>();

            return services.BuildServiceProvider();
        }

        private Jendamark.Messaging.IMessenger InitializeZRE(ConsoleSettingsModel settings, AppSettings appSettings, Serilog.ILogger logger)
        {            
            int appIndex = 1;

            var station = settings.Stations.Single(x => x.StationID == appSettings.StationId);

            var subStation = station.SubStations.Single();

            int stationId = station.StationID;
            int subStationIndex = subStation.SubStationIndex;

            var domains = subStation.ZREDomains.ToHashSet();

            string baseName = $"STN{stationId}SUBSTN{subStationIndex}";
            _zreStarter = new ZREMessagingStarter(logger, baseName, settings.ZRENetworkInterfaceAddress, settings.ZREBroadcastPort
                                                  , settings.ZREBroadcastIntervalInSeconds, domains, appIndex);

            _zreStarter.Start();

            return _zreStarter.Messenger;
        }


        private static Serilog.ILogger InitializeLogger(IConfiguration configuration)
        {
            var seqUrl = configuration["Logging:SeqUrl"]
                ?? throw new InvalidOperationException(
                    "Seq URL is not configured.");

            var logFile = configuration["Logging:LogFile"]
                ?? "Logs\\TacoStationMonitor-.log";

            return new LoggerConfiguration()
                .MinimumLevel.Debug()
                .Enrich.FromLogContext()
                .Enrich.WithProperty("Application", "TacoStationMonitor")
                .WriteTo.Seq(seqUrl)
                .WriteTo.File(
                    logFile,
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();
        }


        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();

            base.OnExit(e);
        }
    }
}
