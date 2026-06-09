using ArcheAgeClassicSteamHelper.Models;

namespace ArcheAgeClassicSteamHelper.Services
{
    public static class
        AAClassicDetectionService
    {
        private const string
            OldLauncherName =
                "ArcheAge Classic Launcher.exe";

        private const string
            NewLauncherName =
                "ArcheAge_Launcher.exe";

        public static string?
            FindGamePath()
        {
            LauncherSettings
                settings =
                    SettingsService
                        .Load();

            /* USER OVERRIDE */

            if (
                settings.GameOverride
                &&
                Directory.Exists(
                    settings.GamePath))
            {
                string? validated =
                    ValidateGamePath(
                        settings.GamePath);

                if (validated
                    != null)
                {
                    return validated;
                }
            }

            /* DEFAULT LOCATION */

            string defaultPath =
                @"C:\Games\AAClassic";

            string? found =
                ValidateGamePath(
                    defaultPath);

            if (found != null)
            {
                SaveDetectedPath(
                    found);

                return found;
            }

            /* STEAM LIBRARIES */

            List<string>
                libraries =
                    SteamDetectionService
                        .GetSteamLibraries();

            foreach (
                string library
                in libraries)
            {
                try
                {
                    string[] exeFiles =
                        Directory.GetFiles(
                            library,
                            OldLauncherName,
                            SearchOption
                                .AllDirectories);

                    if (
                        exeFiles.Length
                        > 0)
                    {
                        string folder =
                            Path
                                .GetDirectoryName(
                                    exeFiles[0])!;

                        RenameLauncher(
                            folder);

                        SaveDetectedPath(
                            folder);

                        return folder;
                    }

                    string[] renamed =
                        Directory.GetFiles(
                            library,
                            NewLauncherName,
                            SearchOption
                                .AllDirectories);

                    if (
                        renamed.Length
                        > 0)
                    {
                        string folder =
                            Path
                                .GetDirectoryName(
                                    renamed[0])!;

                        SaveDetectedPath(
                            folder);

                        return folder;
                    }
                }
                catch
                {
                    /* ignore */
                }
            }

            return null;
        }

        private static string?
            ValidateGamePath(
                string path)
        {
            try
            {
                string oldExe =
                    Path.Combine(
                        path,
                        OldLauncherName);

                string newExe =
                    Path.Combine(
                        path,
                        NewLauncherName);

                if (
                    File.Exists(
                        newExe))
                {
                    return path;
                }

                if (
                    File.Exists(
                        oldExe))
                {
                    RenameLauncher(
                        path);

                    return path;
                }
            }
            catch
            {
                /* ignore */
            }

            return null;
        }

        private static void
            RenameLauncher(
                string folder)
        {
            try
            {
                string oldPath =
                    Path.Combine(
                        folder,
                        OldLauncherName);

                string newPath =
                    Path.Combine(
                        folder,
                        NewLauncherName);

                if (
                    File.Exists(
                        newPath))
                {
                    return;
                }

                if (
                    File.Exists(
                        oldPath))
                {
                    File.Move(
                        oldPath,
                        newPath);
                }
            }
            catch
            {
                /* ignore */
            }
        }

        private static void
            SaveDetectedPath(
                string path)
        {
            try
            {
                LauncherSettings
                    settings =
                        SettingsService
                            .Load();

                if (
                    !settings
                        .GameOverride)
                {
                    settings
                        .GamePath =
                            path;

                    SettingsService
                        .Save(
                            settings);
                }
            }
            catch
            {
                /* ignore */
            }
        }

        public static bool
            IsInstalled()
        {
            return
                FindGamePath()
                != null;
        }
    }
}