#nullable disable

using ArcheAgeClassicSteamHelper.Models;
using ArcheAgeClassicSteamHelper.Services;
using Microsoft.Web.WebView2.WinForms;
using System.Runtime.InteropServices;
using System.Text.Json;
using System.Diagnostics;
using System.Reflection;



namespace ArcheAgeClassicSteamHelper.Forms
{
    public partial class MainForm : Form
    {
        private WebView2 webView;

        private bool
            cachedOwnsArcheAge;

        private bool
            cachedOwnsUnchained;

        private DateTime
            lastOwnershipRefresh =
                DateTime.MinValue;

        [DllImport("gdi32.dll")]
        static extern IntPtr CreateRoundRectRgn(
            int left,
            int top,
            int right,
            int bottom,
            int width,
            int height);

        public MainForm()
        {
            InitializeComponent();

            ConfigureWindow();
            BuildWebView();
        }

        private void ConfigureWindow()
        {
            Text =
                "ArcheAge Classic Steam Helper";

            Size =
                new Size(525, 760);

            MinimumSize =
                new Size(525, 760);

            MaximumSize =
                new Size(820, 760);

            StartPosition =
                FormStartPosition
                    .CenterScreen;

            FormBorderStyle =
                FormBorderStyle.None;

            BackColor =
                Color.Black;

            DoubleBuffered =
                true;

            UpdateRoundedCorners();
        }

        private void
            UpdateRoundedCorners()
        {
            Region =
                Region.FromHrgn(
                    CreateRoundRectRgn(
                        0,
                        0,
                        Width,
                        Height,
                        22,
                        22));
        }

        private async void
            BuildWebView()
        {
            webView =
                new WebView2
                {
                    Dock =
                        DockStyle.Fill,

                    Margin =
                        Padding.Empty
                };

            Controls.Add(
                webView);

            
        await webView
            .EnsureCoreWebView2Async();

            /* =====================================
               EMBEDDED IMAGE SUPPORT
            ===================================== */

            webView
                .CoreWebView2
                .WebResourceRequested +=
                HandleEmbeddedResources;

            webView
                .CoreWebView2
                .AddWebResourceRequestedFilter(
                    "*",
                    Microsoft.Web.WebView2.Core
                        .CoreWebView2WebResourceContext
                        .Image);

            string htmlPath =
                Path.Combine(
                    AppDomain
                        .CurrentDomain
                        .BaseDirectory,
                    "Assets",
                    "launcher.html");



            if (!File.Exists(
                htmlPath))
            {
                MessageBox.Show(
                    $"launcher.html not found:\n\n{htmlPath}",
                    "Launcher Error",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);

                return;
            }

            webView.Source =
                new Uri(
                    htmlPath);

            webView
                .CoreWebView2
                .Settings
                .AreDefaultContextMenusEnabled =
                false;

            webView
                .CoreWebView2
                .Settings
                .AreDevToolsEnabled =
                false;

            webView
                .WebMessageReceived +=
                WebView_WebMessageReceived;

            await Task.Delay(
                1000);

            await SendLauncherStatus();
        }

        private async Task
            SendLauncherStatus()
        {
            try
            {
                LauncherSettings
                    settings =
                        SettingsService
                            .Load();

                /* PATHS */

                string steamPath =
                    settings.SteamOverride
                        ? settings.SteamPath
                        : SteamDetectionService
                            .GetPrimarySteamLibrary()
                        ?? "";

                string gamePath =
                    settings.GameOverride
                        ? settings.GamePath
                        : AAClassicDetectionService
                            .FindGamePath()
                        ?? "";

                /* STATUS */

                bool steamRunning =
                    SteamDetectionService
                        .IsSteamRunning();

                bool gameFound =
                    Directory.Exists(
                        gamePath)
                    &&
                    (
                        File.Exists(
                            Path.Combine(
                                gamePath,
                                "ArcheAge_Launcher.exe"))
                        ||
                        File.Exists(
                            Path.Combine(
                                gamePath,
                                "ArcheAge Classic Launcher.exe"))
                    );

                bool compatibilityReady =
                    steamRunning
                    && gameFound;

                /* OWNERSHIP CACHE */

                if (
                    DateTime.Now
                    >
                    lastOwnershipRefresh
                        .AddSeconds(30))
                {
                    cachedOwnsArcheAge =
                        SteamOwnershipService
                            .OwnsArcheAge();

                    cachedOwnsUnchained =
                        SteamOwnershipService
                            .OwnsArcheAgeUnchained();

                    lastOwnershipRefresh =
                        DateTime.Now;
                }

                var payload =
                    new
                    {
                        steamRunning,

                        gameFound,

                        compatibilityReady,

                        steamPath,

                        gamePath,

                        steamOverride =
                            settings
                                .SteamOverride,

                        gameOverride =
                            settings
                                .GameOverride,

                        preferredSteamVersion =
                            settings
                                .PreferredSteamVersion,

                        ownsArcheAge =
                            cachedOwnsArcheAge,

                        ownsUnchained =
                            cachedOwnsUnchained
                    };

                string json =
                    JsonSerializer
                        .Serialize(
                            payload);

                webView
                    .CoreWebView2
                    .PostWebMessageAsJson(
                        json);
            }
            catch
            {
                /* ignore */
            }

            await Task.CompletedTask;
        }

      
private void
    HandleEmbeddedResources(
        object sender,
        Microsoft.Web.WebView2.Core
            .CoreWebView2WebResourceRequestedEventArgs e)
        {
            string uri =
                e.Request.Uri
                    .ToLower();

            string? resourceName =
                uri switch
                {
                    var u when u.Contains(
                        "logo.png")
                        =>
                        "ArcheAgeClassicSteamHelper.Assets.images.logo.png",

                    var u when u.Contains(
                        "discord.png")
                        =>
                        "ArcheAgeClassicSteamHelper.Assets.images.discord.png",

                    var u when u.Contains(
                        "wiki.png")
                        =>
                        "ArcheAgeClassicSteamHelper.Assets.images.wiki.png",

                    var u when u.Contains(
                        "play-button.png")
                        =>
                        "ArcheAgeClassicSteamHelper.Assets.images.play-button.png",

                    var u when u.Contains(
                        "background.png")
                        =>
                        "ArcheAgeClassicSteamHelper.Assets.images.background.png",

                    _ => null
                };

            if (
                resourceName
                is null)
            {
                return;
            }

            Stream? stream =
                Assembly
                    .GetExecutingAssembly()
                    .GetManifestResourceStream(
                        resourceName);

            if (
                stream is null)
            {
                return;
            }

            e.Response =
                webView
                    .CoreWebView2
                    .Environment
                    .CreateWebResourceResponse(
                        stream,
                        200,
                        "OK",
                        "Content-Type: image/png");
        }


        
private void
    WebView_WebMessageReceived(
        object sender,
        Microsoft.Web.WebView2
        .Core
        .CoreWebView2WebMessageReceivedEventArgs e)
        {
            string message =
                e.TryGetWebMessageAsString();

            /* =====================================
               OPEN URL (DISCORD / WIKI)
            ===================================== */

            if (
                message.StartsWith("{"))
            {
                try
                {
                    using JsonDocument json =
                        JsonDocument.Parse(
                            message);

                    string? type =
                        json.RootElement
                            .GetProperty(
                                "type")
                            .GetString();

                    if (
                        type ==
                        "openUrl")
                    {
                        string? url =
                            json.RootElement
                                .GetProperty(
                                    "url")
                                .GetString();

                        if (
                            !string.IsNullOrWhiteSpace(
                                url))
                        {
                            Process.Start(
                                new ProcessStartInfo
                                {
                                    FileName =
                                        url,

                                    UseShellExecute =
                                        true
                                });
                        }

                        return;
                    }
                }
                catch
                {
                    // ignore invalid json
                }
            }

            switch (message)
            {
                case "dragWindow":

                    NativeMethods
                        .ReleaseCapture();

                    NativeMethods
                        .SendMessage(
                            Handle,
                            0xA1,
                            0x2,
                            0);

                    break;

                case "close":

                    Close();

                    break;

                case "minimize":

                    WindowState =
                        FormWindowState
                            .Minimized;

                    break;

                case "expandLauncher":

                    Size =
                        new Size(
                            820,
                            760);

                    UpdateRoundedCorners();

                    break;

                case "collapseLauncher":

                    Size =
                        new Size(
                            525,
                            760);

                    UpdateRoundedCorners();

                    break;

                case "refreshStatus":

                    _ =
                        SendLauncherStatus();

                    break;

                case var msg
                    when msg.StartsWith(
                        "setSteamVersion:"):

                    {
                        string version =
                            msg.Replace(
                                "setSteamVersion:",
                                "");

                        LauncherSettings
                            settings =
                                SettingsService
                                    .Load();

                        settings
                            .PreferredSteamVersion =
                                version;

                        SettingsService
                            .Save(
                                settings);

                        _ =
                            SendLauncherStatus();

                        break;
                    }

                case "play":

                    _ =
                        PlayService
                            .Launch();

                    break;
            }
        }


    }
}

namespace ArcheAgeClassicSteamHelper
{
    internal static class NativeMethods
    {
        [DllImport(
            "user32.dll")]
        internal static extern bool
            ReleaseCapture();

        [DllImport(
            "user32.dll")]
        internal static extern IntPtr
            SendMessage(
                IntPtr hWnd,
                int Msg,
                int wParam,
                int lParam);
    }
}

