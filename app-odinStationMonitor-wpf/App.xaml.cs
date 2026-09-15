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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Load application configuration
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile(
                    "appsettings.json",
                    optional: false,
                    reloadOnChange: true)
                .Build();

            // Get LIC configuration
            var licOptions = new LicOptions();
            configuration.GetSection("LIC").Bind(licOptions);

            // Read ODIN configuration from LIC
            var settingsService = new ConsoleSettingsService();

            ConsoleSettingsModel settings =
                settingsService.GetSettings(licOptions.DatabasePath);

            // Configure DI
            _serviceProvider = InitializeServices(
                configuration,
                settings);

            // We'll resolve and show MainWindow here later
        }

        private static ServiceProvider InitializeServices(IConfiguration configuration, ConsoleSettingsModel settings)
        {
            var services = new ServiceCollection();

            // Application configuration
            services.AddSingleton(configuration);

            // LIC settings - loaded once for application lifetime
            services.AddSingleton(settings);

            // LIC
            services.AddSingleton<IConsoleSettingsService, ConsoleSettingsService>();

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

        protected override void OnExit(ExitEventArgs e)
        {
            _serviceProvider?.Dispose();

            base.OnExit(e);
        }
    }
}
