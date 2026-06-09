using System.Diagnostics;

namespace ArcheAgeClassicSteamHelper.Services
{
    public static class SteamManifestService
    {
        public static bool
            PrepareSteamGame(
                string appId,
                string gamePath,
                string steamAppsPath)
        {
            try
            {
                string breadcrumb =
                    Path.Combine(
                        gamePath,
                        $"steam_setup_{appId}.txt");

                /* ALREADY CONFIGURED */

                if (
                    File.Exists(
                        breadcrumb))
                {
                    return true;
                }

                string gameFolderName =
                    GetSteamFolderName(
                        appId);

                string commonPath =
                    Path.Combine(
                        steamAppsPath,
                        "common");

                Directory.CreateDirectory(
                    commonPath);

                string steamGameFolder =
                    Path.Combine(
                        commonPath,
                        gameFolderName);

                /* SETUP */

                EnsureSymlink(
                    steamGameFolder,
                    gamePath);

                CreateManifest(
                    appId,
                    steamAppsPath,
                    gameFolderName);

                CreateSteamAppId(
                    gamePath,
                    appId);

                /* REMOVE OLD VERSION BREADCRUMB */

                RemoveOppositeBreadcrumb(
                    gamePath,
                    appId);

                /* CREATE BREADCRUMB */

                File.WriteAllText(
                    breadcrumb,
                    DateTime.Now
                        .ToString(
                            "yyyy-MM-dd HH:mm:ss"));

                /* RESTART STEAM ONCE */

                RestartSteam();

                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void
            RemoveOppositeBreadcrumb(
                string gamePath,
                string appId)
        {
            try
            {
                string oppositeAppId =
                    appId
                    ==
                    "304030"
                        ? "1147660"
                        : "304030";

                string opposite =
                    Path.Combine(
                        gamePath,
                        $"steam_setup_{oppositeAppId}.txt");

                if (
                    File.Exists(
                        opposite))
                {
                    File.Delete(
                        opposite);
                }
            }
            catch
            {
            }
        }

        private static string
            GetSteamFolderName(
                string appId)
        {
            return appId switch
            {
                "304030" =>
                    "ArcheAge",

                "1147660" =>
                    "ArcheAge 1147660",

                _ =>
                    "ArcheAge"
            };
        }

        private static void
            EnsureSymlink(
                string linkPath,
                string targetPath)
        {
            try
            {
                if (
                    Directory.Exists(
                        linkPath))
                {
                    return;
                }

                Process.Start(
                    new ProcessStartInfo
                    {
                        FileName =
                            "cmd.exe",

                        Arguments =
                            $"/c mklink /D \"{linkPath}\" \"{targetPath}\"",

                        Verb =
                            "runas",

                        WindowStyle =
                            ProcessWindowStyle
                                .Hidden,

                        CreateNoWindow =
                            true
                    })?
                    .WaitForExit();
            }
            catch
            {
            }
        }

        private static void
            CreateSteamAppId(
                string gamePath,
                string appId)
        {
            try
            {
                string file =
                    Path.Combine(
                        gamePath,
                        "steam_appid.txt");

                File.WriteAllText(
                    file,
                    appId);
            }
            catch
            {
            }
        }

        private static void
            CreateManifest(
                string appId,
                string steamAppsPath,
                string installDir)
        {
            try
            {
                string manifestPath =
                    Path.Combine(
                        steamAppsPath,
                        $"appmanifest_{appId}.acf");

                string manifest =
$@"
""AppState""
{{
    ""appid""        ""{appId}""
    ""Universe""     ""1""
    ""LauncherPath"" ""steam.exe""
    ""name""         ""ArcheAge Classic""
    ""StateFlags""   ""4""
    ""installdir""   ""{installDir}""
    ""LastUpdated""  ""0""
    ""UpdateResult"" ""0""
    ""SizeOnDisk""   ""0""
    ""buildid""      ""1""
}}
";

                File.WriteAllText(
                    manifestPath,
                    manifest);
            }
            catch
            {
            }
        }

        public static void
            RestartSteam()
        {
            try
            {
                foreach (
                    Process process
                    in Process
                        .GetProcessesByName(
                            "steam"))
                {
                    try
                    {
                        process.Kill();
                    }
                    catch
                    {
                    }
                }

                Thread.Sleep(
                    2500);

                string? steamExe =
                    SteamDetectionService
                        .GetSteamPath();

                if (
                    !string.IsNullOrWhiteSpace(
                        steamExe))
                {
                    Process.Start(
                        Path.Combine(
                            steamExe,
                            "steam.exe"));
                }

                Thread.Sleep(
                    5000);
            }
            catch
            {
            }
        }
    }
}

