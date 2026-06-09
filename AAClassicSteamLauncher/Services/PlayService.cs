using System.Diagnostics;
using ArcheAgeClassicSteamHelper.Models;

namespace ArcheAgeClassicSteamHelper.Services
{
    public static class PlayService
    {
        private const string
            OldLauncherName =
                "ArcheAge Classic Launcher.exe";

        private const string
            NewLauncherName =
                "ArcheAge_Launcher.exe";

        public static async Task
            Launch()
        {
            try
            {
                LauncherSettings
                    settings =
                        SettingsService
                            .Load();

                /* FIND GAME */

                string? gamePath =
                    settings.GameOverride
                        ? settings.GamePath
                        : AAClassicDetectionService
                            .FindGamePath();

                if (
                    string.IsNullOrWhiteSpace(
                        gamePath))
                {
                    MessageBox.Show(
                        "ArcheAge Classic installation could not be found.",
                        "Launcher Error",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);

                    return;
                }

                /* RENAME EXE */

                RenameLauncher(
                    gamePath);

                /* DETECT STEAM */

                string? steamAppsPath =
                    settings.SteamOverride
                        ? settings.SteamPath
                        : SteamDetectionService
                            .GetPrimarySteamLibrary();

                if (
                    string.IsNullOrWhiteSpace(
                        steamAppsPath))
                {
                    LaunchDirect(
                        gamePath);

                    return;
                }

                /* DETERMINE VERSION */

                string? appId =
                    ResolveAppId(
                        settings
                            .PreferredSteamVersion);

                /* NO OWNERSHIP */

                if (
                    string.IsNullOrWhiteSpace(
                        appId))
                {
                    LaunchDirect(
                        gamePath);

                    return;
                }

                /* PREPARE STEAM */

                bool prepared =
                    SteamManifestService
                        .PrepareSteamGame(
                            appId,
                            gamePath,
                            steamAppsPath);

                if (!prepared)
                {
                    LaunchDirect(
                        gamePath);

                    return;
                }

              

                bool launched =
                    LaunchSteam(
                       appId);


                if (!launched)
                {
                    LaunchDirect(
                        gamePath);
                }

                await Task.CompletedTask;
            }
            catch
            {
                try
                {
                    string? path =
                        AAClassicDetectionService
                            .FindGamePath();

                    if (
                        !string.IsNullOrWhiteSpace(
                            path))
                    {
                        LaunchDirect(
                            path);
                    }
                }
                catch
                {
                }
            }
        }

        private static string?
            ResolveAppId(
                string preferredVersion)
        {
            bool ownsAA =
                SteamOwnershipService
                    .OwnsArcheAge();

            bool ownsUnchained =
                SteamOwnershipService
                    .OwnsArcheAgeUnchained();

            /* MANUAL OVERRIDE */

            switch (
                preferredVersion)
            {
                case "ArcheAge":

                    return ownsAA
                        ? SteamOwnershipService
                            .ArcheAgeAppId
                        : null;

                case "Unchained":

                    return ownsUnchained
                        ? SteamOwnershipService
                            .ArcheAgeUnchainedAppId
                        : null;
            }

            /* AUTO DETECT */

            if (
                ownsAA)
            {
                return
                    SteamOwnershipService
                        .ArcheAgeAppId;
            }

            if (
                ownsUnchained)
            {
                return
                    SteamOwnershipService
                        .ArcheAgeUnchainedAppId;
            }

            return null;
        }

        private static void
            RenameLauncher(
                string gamePath)
        {
            try
            {
                string oldExe =
                    Path.Combine(
                        gamePath,
                        OldLauncherName);

                string newExe =
                    Path.Combine(
                        gamePath,
                        NewLauncherName);

                if (
                    File.Exists(
                        oldExe)
                    &&
                    !File.Exists(
                        newExe))
                {
                    File.Move(
                        oldExe,
                        newExe);
                }
            }
            catch
            {
            }
        }

        private static bool
            LaunchSteam(
                string appId)
        {
            try
            {
                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            $"steam://rungameid/{appId}",

                        UseShellExecute =
                            true
                    });

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void
            LaunchDirect(
                string gamePath)
        {
            try
            {
                string launcher =
                    Path.Combine(
                        gamePath,
                        NewLauncherName);

                if (
                    !File.Exists(
                        launcher))
                {
                    launcher =
                        Path.Combine(
                            gamePath,
                            OldLauncherName);
                }

                if (
                    File.Exists(
                        launcher))
                {
                    Process.Start(
                        new ProcessStartInfo
                        {
                            FileName =
                                launcher,

                            WorkingDirectory =
                                gamePath,

                            UseShellExecute =
                                true
                        });
                }
            }
            catch
            {
            }
        }
    }
}

