# Battle.Net Installer

[Русская версия](README.ru.md)

Simple tool for installing, updating, or repairing Blizzard games through the locally installed Battle.net Agent.

This repository is an unofficial maintenance fork of [barncastle/Battle.Net-Installer](https://github.com/barncastle/Battle.Net-Installer) and includes a maintained fix for `Error 2310`.

## Launch Tips

- Install Battle.net and sign in at least once.
- Download the release `BNetInstaller.exe` and keep it somewhere easy to find.
- Double-click `BNetInstaller.exe`, then enter the requested values in the console.
- Use the install folder you actually want for the game, then let the tool handle the rest.

## Download

Download the latest `BNetInstaller.exe` from [Releases](https://github.com/DokPlay/Battle.Net-Installer/releases/latest).

## Quick Start

1. Install Battle.net.
2. Sign in to your Battle.net account once.
3. Download `BNetInstaller.exe`.
4. Double-click `BNetInstaller.exe`.
5. Enter the values the program asks for.
6. Wait for the download or install progress to appear.

## When You Open the Downloaded File

The program opens a console window and asks for values one by one. A typical run looks like this:

```text
Please complete the following information:
TACT Product (example: s2): fenris
Agent UID (example: s2_enUS, blank = same as product): fenris
Installation Directory (example: D:\Battle.net\StarCraft II): D:\Diablo IV
Game/Asset Language (example: enUS): ruRU
Repair Install? (Y/N, default N): n
```

What this means:

- `fenris` is the value for `TACT Product (example: s2)`.
- `fenris` is also the value for `Agent UID (example: s2_enUS, blank = same as product)`.
- `D:\Diablo IV` is the value for `Installation Directory (example: D:\Battle.net\StarCraft II)`. You can use another drive or folder, for example `C:\Diablo IV` or `E:\Games\Diablo IV`.
- `ruRU` is the value for `Game/Asset Language (example: enUS)`. Other examples are `enUS`, `deDE`, `frFR`, `esES`, `ptBR`, `itIT`, `koKR`, `plPL`, `zhCN`, and `zhTW`.
- `n` is the value for `Repair Install? (Y/N, default N)` and means no repair, because the game is not installed yet.

## Installation Flow

```mermaid
flowchart TB
    subgraph FLOW[" "]
        direction LR

        subgraph LEFT["Prepare"]
            direction TB
            A["Download BNetInstaller.exe"]
            B["Open the downloaded file"]
            C["Enter product, UID, install folder, language, and repair choice"]
            A --> B
            B --> C
        end

        subgraph RIGHT["Run"]
            direction TB
            D["Tool opens a local Battle.net Agent session"]
            E["Tool queues install, update, or repair"]
            F["Progress appears in the console"]
            D --> E
            E --> F
        end
    end
```

## After Installation

This installer only handles the install/update flow. What appears in the Battle.net launcher is controlled by Battle.net itself, your account, and your region.

> [!IMPORTANT]
> If Diablo IV does not appear in Battle.net after installation, some users additionally rely on unofficial third-party utilities, for example `Blizzless Unlocker`.

1. Open the Battle.net app and go to `Diablo IV`.
2. Click the gear icon next to `Play`.
3. Open `Game Settings`.
4. Choose the language you want for `Text Language`.
5. Choose the language you want for `Spoken Language` if needed.
6. Click `Done` and wait for Battle.net to download the required language files.

## Requirements

- Windows
- Battle.net installed
- Signed in to Battle.net at least once

If you are using the release `EXE`, you do not need to install the .NET runtime separately because the release build is self-contained.

## Note

Use this tool only with products that are already available to your Blizzard account.

## Attribution

Original project: [barncastle/Battle.Net-Installer](https://github.com/barncastle/Battle.Net-Installer)
Error 2310 fix reference: [xCortlandx/Battle.Net-Installer](https://github.com/xCortlandx/Battle.Net-Installer)
