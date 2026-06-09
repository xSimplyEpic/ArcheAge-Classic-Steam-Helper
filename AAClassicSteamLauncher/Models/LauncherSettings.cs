namespace ArcheAgeClassicSteamHelper.Models
{
    public class LauncherSettings
    {
        public string SteamPath
        {
            get;
            set;
        } = string.Empty;

        public string GamePath
        {
            get;
            set;
        } = string.Empty;

        public bool SteamOverride
        {
            get;
            set;
        }

        public bool GameOverride
        {
            get;
            set;
        }

        /*
            Auto
            ArcheAge
            Unchained
        */

        public string
            PreferredSteamVersion
        {
            get;
            set;
        } = "Auto";
    }
}

