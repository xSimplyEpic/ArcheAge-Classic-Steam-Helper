using Microsoft.Win32;
using System.Diagnostics;
using System.Text.RegularExpressions;

namespace ArcheAgeClassicSteamHelper.Services
{
    public static class SteamDetectionService
    {
        public static string? GetSteamPath()
        {
            try
            {
                using RegistryKey? key =
                    Registry.CurrentUser.OpenSubKey(
                        @"Software\Valve\Steam");

                return key?
                    .GetValue("SteamPath")
                    ?.ToString();
            }
            catch
            {
                return null;
            }
        }

       
        public static string? GetPrimarySteamLibrary()
        {
            try
            {
                List<string> libraries =
                    GetSteamLibraries();

                string? library =
                    libraries
                        .FirstOrDefault();

                if (
                    string.IsNullOrWhiteSpace(
                        library))
                {
                    return null;
                }

                string steamAppsPath =
                    Path.Combine(
                        library,
                        "steamapps");

                if (
                    Directory.Exists(
                        steamAppsPath))
                {
                    return steamAppsPath;
                }

                return library;
            }
            catch
            {
                return null;
            }
        }


        public static List<string> GetSteamLibraries()
        {
            List<string> libraries =
                new();

            string? steamInstallPath =
                GetSteamPath();

            if (string.IsNullOrWhiteSpace(
                steamInstallPath))
            {
                return libraries;
            }

            string libraryFile =
                Path.Combine(
                    steamInstallPath,
                    "steamapps",
                    "libraryfolders.vdf");

            /* Try reading real Steam libraries */

            if (File.Exists(
                libraryFile))
            {
                try
                {
                    string content =
                        File.ReadAllText(
                            libraryFile);

                    MatchCollection matches =
                        Regex.Matches(
                            content,
                            "\"path\"\\s+\"([^\"]+)\"");

                    foreach (
                        Match match
                        in matches)
                    {
                        string path =
                            match
                                .Groups[1]
                                .Value
                                .Replace(
                                    @"\\",
                                    @"\");

                        if (
                            Directory.Exists(path)
                            &&
                            !libraries.Contains(path))
                        {
                            libraries.Add(
                                path);
                        }
                    }
                }
                catch
                {
                }
            }

            /* Fallback to Steam install folder */

            if (
                libraries.Count == 0
                &&
                Directory.Exists(
                    steamInstallPath))
            {
                libraries.Add(
                    steamInstallPath);
            }

            return libraries;
        }

        public static bool IsSteamRunning()
        {
            return Process
                .GetProcessesByName(
                    "steam")
                .Any();
        }
    }
}

