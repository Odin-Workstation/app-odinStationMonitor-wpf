using Jendamark.ODINWorkStationV2.LocalInformationCache.InformationAccess;
using Jendamark.ODINWorkStationV2.LocalInformationCache.Models;

namespace TacoStationMonitor.Services
{
    public sealed class ConsoleSettingsService : IConsoleSettingsService
    {
        public ConsoleSettingsModel GetSettings(string licDatabasePath)
        {
            return ConsoleLIC.GetSettingsWithoutContainer(licDatabasePath)
                ?? throw new InvalidOperationException(
                    $"Could not read console settings from LIC at '{licDatabasePath}'.");
        }
    }
}
