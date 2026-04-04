# Battle.Net Installer

[English](README.md) | [Русская версия](README.ru.md)

Simple tool for installing, updating, or repairing Blizzard games through the locally installed Battle.net Agent.

This repository is an unofficial maintenance fork of [barncastle/Battle.Net-Installer](https://github.com/barncastle/Battle.Net-Installer) and includes a maintained fix for `Error 2310`.

## Download

Download the latest `BNetInstaller.exe` from [Releases](https://github.com/DokPlay/Battle.Net-Installer/releases/latest).

## Quick Start

1. Install Battle.net.
2. Sign in to your Battle.net account once.
3. Download `BNetInstaller.exe`.
4. Double-click `BNetInstaller.exe`.
5. Enter the values the program asks for.
6. Wait for the download or install progress to appear.

## If You Double-Click the EXE

The program opens a console window and asks for:

1. `TACT Product`
2. `Agent UID`
3. `Installation Directory`
4. `Game/Asset Language`
5. `Repair Install? (Y/N)`

Example:

```text
Please complete the following information:
TACT Product (example: s2): fenris
Agent UID (example: s2_enUS, blank = same as product): fenris
Installation Directory (example: D:\Battle.net\StarCraft II): D:\Diablo IV
Game/Asset Language (example: enUS): ruRU
Repair Install? (Y/N, default N): n
```

Notes:

- In this example, `D:\Diablo IV` was used.
- You can use another drive, for example `C:\Diablo IV`, `E:\Games\Diablo IV`, or any other path.
- You can also change `Diablo IV` to any folder name you want.
- If `Agent UID` is the same as the product, you can leave it blank and press `Enter`.
- The game folder is chosen here in the console input, not in the Battle.net launcher.

## Command Line Example

If you prefer running it manually from the terminal:

```bat
BNetInstaller.exe --prod fenris --uid fenris --lang ruRU --dir "D:\Diablo IV"
```

You can change:

- `fenris` to another product
- `ruRU` to another locale
- `D:\Diablo IV` to any drive and folder

## Error 2310

If you get `Error 2310`:

1. Make sure you are signed in to Battle.net.
2. Run the tool again with the same folder.
3. If the error still appears, open Battle.net and try `Locate the game` for that same folder.
4. Then run `BNetInstaller.exe` again.

## Requirements

- Windows
- Battle.net installed
- Signed in to Battle.net at least once

If you are using the release `EXE`, you do not need to install the .NET runtime separately because the release build is self-contained.

## Build From Source

If you want to build it yourself:

```powershell
git clone https://github.com/DokPlay/Battle.Net-Installer.git
cd Battle.Net-Installer
dotnet publish BNetInstaller\BNetInstaller.csproj -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

Output:

```text
BNetInstaller\bin\Release\net8.0\win-x64\publish\BNetInstaller.exe
```

## Note

Use this tool only with products that are already available to your Blizzard account.

## Attribution

Original project: [barncastle/Battle.Net-Installer](https://github.com/barncastle/Battle.Net-Installer)
