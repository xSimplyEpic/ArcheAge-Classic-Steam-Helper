using ArcheAgeClassicSteamHelper.Models;
using System.Text.Json;

namespace ArcheAgeClassicSteamHelper.Services
{
    public static class SettingsService
    {
        private static readonly string
            SettingsFolder =
                Path.Combine(
                    Environment
                        .GetFolderPath(
                            Environment
                                .SpecialFolder
                                .MyDocuments),
                    "AACSteamLauncher");

        private static readonly string
            SettingsPath =
                Path.Combine(
                    SettingsFolder,
                    "settings.json");

        public static LauncherSettings
            Load()
        {
            try
            {
                EnsureFolderExists();

                if (!File.Exists(
                    SettingsPath))
                {
                    return new
                        LauncherSettings();
                }

                string json =
                    File.ReadAllText(
                        SettingsPath);

                LauncherSettings?
                    settings =
                        JsonSerializer
                            .Deserialize
                                <LauncherSettings>(
                                    json);

                return settings
                    ??
                    new LauncherSettings();
            }
            catch
            {
                return new
                    LauncherSettings();
            }
        }

        public static void Save(
            LauncherSettings settings)
        {
            try
            {
                EnsureFolderExists();

                string json =
                    JsonSerializer
                        .Serialize(
                            settings,
                            new JsonSerializerOptions
                            {
                                WriteIndented = true
                            });

                File.WriteAllText(
                    SettingsPath,
                    json);
            }
            catch
            {
                /* ignore */
            }
        }

        private static void
            EnsureFolderExists()
        {
            if (!Directory.Exists(
                SettingsFolder))
            {
                Directory.CreateDirectory(
                    SettingsFolder);
            }
        }
    }
}