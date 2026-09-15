using Jendamark.ODINWorkStationV2.LocalInformationCache.Models;

namespace TacoStationMonitor.Services
{
    public interface IConsoleSettingsService
    {
        ConsoleSettingsModel GetSettings(string licDatabasePath);
    }
}