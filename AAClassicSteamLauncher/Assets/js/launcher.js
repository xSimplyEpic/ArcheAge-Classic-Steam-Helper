let settingsOpen = false;

function refreshLauncherStatus() {

    window.chrome.webview.postMessage(
        "refreshStatus"
    );
}

document.addEventListener(
    "DOMContentLoaded",
    () => {

        /* =====================================
           ELEMENTS
        ===================================== */

        const topbar =
            document.querySelector(
                ".topbar"
            );

        const settingsBtn =
            document.getElementById(
                "settingsBtn"
            );

        const closeBtn =
            document.getElementById(
                "closeBtn"
            );

        const minBtn =
            document.getElementById(
                "minBtn"
            );

        const discordBtn =
            document.getElementById(
                "discordBtn"
            );

        const wikiBtn =
            document.getElementById(
                "wikiBtn"
            );

        const playBtn =
            document.getElementById(
                "playBtn"
            );

        const steamPathInput =
            document.getElementById(
                "steamPath"
            );

        const gamePathInput =
            document.getElementById(
                "gamePath"
            );

        const steamStatus =
            document.getElementById(
                "steamDetectStatus"
            );

        const gameStatus =
            document.getElementById(
                "gameDetectStatus"
            );

        const steamVersionSelect =
            document.getElementById(
                "steamVersionSelect"
            );

        const aaDot =
            document.getElementById(
                "aaDot"
            );

        const unchainedDot =
            document.getElementById(
                "unchainedDot"
            );

        const compatibilitySub =
            document.getElementById(
                "compatibilitySub"
            );

        const footerAppId =
            document.getElementById(
                "footerAppId"
            );

        /* =====================================
           DRAG WINDOW
        ===================================== */

        topbar?.addEventListener(
            "mousedown",
            event => {

                const clickedButton =
                    event.target.closest(
                        "button"
                    );

                if (
                    clickedButton
                ) {
                    return;
                }

                window.chrome
                    .webview
                    .postMessage(
                        "dragWindow"
                    );
            }
        );

        /* =====================================
           PLAY
        ===================================== */

        playBtn?.addEventListener(
            "click",
            () => {

                window.chrome
                    .webview
                    .postMessage(
                        "play"
                    );
            }
        );

        /* =====================================
           DISCORD
        ===================================== */

        discordBtn
            ?.addEventListener(
                "click",
                event => {

                    event
                        .stopPropagation();

                    window.chrome
                        .webview
                        .postMessage(
                            JSON.stringify(
                            {
                                type:
                                    "openUrl",

                                url:
                                    "https://discord.com/invite/aaclassic"
                            })
                        );
                }
            );

        /* =====================================
           WIKI
        ===================================== */

        wikiBtn
            ?.addEventListener(
                "click",
                event => {

                    event
                        .stopPropagation();

                    window.chrome
                        .webview
                        .postMessage(
                            JSON.stringify(
                            {
                                type:
                                    "openUrl",

                                url:
                                    "https://wiki.aa-classic.com/Main_Page"
                            })
                        );
                }
            );

        /* =====================================
           SAVE DROPDOWN
        ===================================== */

        steamVersionSelect
            ?.addEventListener(
                "change",
                () => {

                    const value =
                        steamVersionSelect
                            .value;

                    window.chrome
                        .webview
                        .postMessage(
                            `setSteamVersion:${value}`
                        );

                    updateCompatibilityText(
                        value
                    );
                }
            );

        function
            updateCompatibilityText(
                value)
        {
            let mode =
                "Auto Detect";

            switch (
                value)
            {
                case "ArcheAge":

                    mode =
                        "ArcheAge";

                    break;

                case "Unchained":

                    mode =
                        "ArcheAge: Unchained";

                    break;
            }

            if (
                compatibilitySub
            ) {

                compatibilitySub
                    .textContent =
                    `Steam Compatibility: ${mode}`;
            }

            if (
                footerAppId
            ) {

                footerAppId
                    .textContent =
                    `Steam Compatibility: ${mode}`;
            }
        }

        /* =====================================
           SETTINGS PANEL
        ===================================== */

        settingsBtn
            ?.addEventListener(
                "click",
                () => {

                    settingsOpen =
                        !settingsOpen;

                    document.body
                        .classList
                        .toggle(
                            "settings-open"
                        );

                    window.chrome
                        .webview
                        .postMessage(
                            settingsOpen
                                ? "expandLauncher"
                                : "collapseLauncher"
                        );
                }
            );

        /* =====================================
           CLOSE
        ===================================== */

        closeBtn
            ?.addEventListener(
                "click",
                event => {

                    event
                        .stopPropagation();

                    window.chrome
                        .webview
                        .postMessage(
                            "close"
                        );
                }
            );

        /* =====================================
           MINIMIZE
        ===================================== */

        minBtn
            ?.addEventListener(
                "click",
                event => {

                    event
                        .stopPropagation();

                    window.chrome
                        .webview
                        .postMessage(
                            "minimize"
                        );
                }
            );

        /* =====================================
           AUTO REFRESH
        ===================================== */

        refreshLauncherStatus();

        setInterval(
            refreshLauncherStatus,
            5000
        );

        /* =====================================
           RECEIVE STATUS
        ===================================== */

        window.chrome
            .webview
            .addEventListener(
                "message",
                event => {

                    const data =
                        event.data;

                    const ready =
                        document
                            .querySelectorAll(
                                ".ready"
                            );

                    if (
                        ready.length >= 3
                    ) {

                        ready[0]
                            .textContent =
                            data.steamRunning
                                ? "READY"
                                : "NOT RUNNING";

                        ready[0]
                            .style.color =
                            data.steamRunning
                                ? "#63e65b"
                                : "#ffb347";

                        ready[1]
                            .textContent =
                            data.gameFound
                                ? "READY"
                                : "NOT FOUND";

                        ready[1]
                            .style.color =
                            data.gameFound
                                ? "#63e65b"
                                : "#ff6666";

                        ready[2]
                            .textContent =
                            data.compatibilityReady
                                ? "ACTIVE"
                                : "WAITING";
                    }

                    if (
                        steamVersionSelect
                        &&
                        data
                            .preferredSteamVersion
                    ) {

                        steamVersionSelect
                            .value =
                            data
                                .preferredSteamVersion;
                    }

                    updateCompatibilityText(
                        data
                            .preferredSteamVersion
                    );

                    if (
                        steamPathInput
                    ) {

                        steamPathInput
                            .value =
                            data
                                .steamPath
                            || "";

                        steamPathInput
                            .title =
                            data
                                .steamPath
                            || "";
                    }

                    if (
                        gamePathInput
                    ) {

                        gamePathInput
                            .value =
                            data
                                .gamePath
                            || "";

                        gamePathInput
                            .title =
                            data
                                .gamePath
                            || "";
                    }

                    aaDot
                        ?.classList
                        .toggle(
                            "owned",
                            data
                                .ownsArcheAge
                        );

                    unchainedDot
                        ?.classList
                        .toggle(
                            "owned",
                            data
                                .ownsUnchained
                        );

                    if (
                        steamStatus
                    ) {

                        steamStatus
                            .textContent =
                            data
                                .steamOverride
                                ? "⚙ Manual Override"
                                : "✓ Auto Detected";
                    }

                    if (
                        gameStatus
                    ) {

                        gameStatus
                            .textContent =
                            data
                                .gameOverride
                                ? "⚙ Manual Override"
                                : "✓ Auto Detected";
                    }
                }
            );
    }
);
