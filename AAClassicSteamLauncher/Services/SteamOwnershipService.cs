using System.Text.RegularExpressions;

namespace ArcheAgeClassicSteamHelper.Services
{
    public static class SteamOwnershipService
    {
        public const string
            ArcheAgeAppId =
                "304030";

        public const string
            ArcheAgeUnchainedAppId =
                "1147660";

        /*
            Returns preferred game:

            304030
            1147660
            null
        */

        public static string?
            GetPreferredOwnedGame()
        {
            try
            {
                bool ownsArcheAge =
                    OwnsArcheAge();

                bool ownsUnchained =
                    OwnsArcheAgeUnchained();

                /* priority */

                if (
                    ownsArcheAge)
                {
                    return
                        ArcheAgeAppId;
                }

                if (
                    ownsUnchained)
                {
                    return
                        ArcheAgeUnchainedAppId;
                }
            }
            catch
            {
                /* ignore */
            }

            return null;
        }

        public static bool
            OwnsArcheAge()
        {
            return ScanOwnership(
                ArcheAgeAppId);
        }

        public static bool
            OwnsArcheAgeUnchained()
        {
            return ScanOwnership(
                ArcheAgeUnchainedAppId);
        }

        private static bool
            ScanOwnership(
                string appId)
        {
            try
            {
                string? steamPath =
                    SteamDetectionService
                        .GetSteamPath();

                if (
                    string.IsNullOrWhiteSpace(
                        steamPath))
                {
                    return false;
                }

                string userdataPath =
                    Path.Combine(
                        steamPath,
                        "userdata");

                if (
                    !Directory.Exists(
                        userdataPath))
                {
                    return false;
                }

                /*
                    Scan ALL Steam
                    accounts once.

                    If ANY account
                    owns the game,
                    return true.
                */

                string[] configFiles =
                    Directory.GetFiles(
                        userdataPath,
                        "localconfig.vdf",
                        SearchOption
                            .AllDirectories);

                foreach (
                    string config
                    in configFiles)
                {
                    try
                    {
                        string content =
                            File.ReadAllText(
                                config);

                        if (
                            OwnsGame(
                                content,
                                appId))
                        {
                            return true;
                        }
                    }
                    catch
                    {
                        /* ignore */
                    }
                }
            }
            catch
            {
                /* ignore */
            }

            return false;
        }

        private static bool
            OwnsGame(
                string content,
                string appId)
        {
            try
            {
                return Regex.IsMatch(
                    content,
                    $"\"{appId}\"",
                    RegexOptions
                        .IgnoreCase);
            }
            catch
            {
                return false;
            }
        }
    }
}

