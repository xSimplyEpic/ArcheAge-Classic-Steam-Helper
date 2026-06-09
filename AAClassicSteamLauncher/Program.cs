using ArcheAgeClassicSteamHelper.Forms;

namespace ArcheAgeClassicSteamHelper
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            Application.Run(new MainForm());
        }
    }
}