# Battle.Net Installer

A tool for installing, updating and repairing games via Blizzard's Battle.net application. This tool can either be run directly or via command line using the below arguments.

Windows only.

## Maintenance Fork

This repository is an unofficial maintenance fork of [barncastle/Battle.Net-Installer](https://github.com/barncastle/Battle.Net-Installer).

- Upstream project: [barncastle/Battle.Net-Installer](https://github.com/barncastle/Battle.Net-Installer)
- This fork keeps the original project history and attribution intact
- This fork includes a maintained fix for Battle.net `Error 2310` and related install-flow improvements

At the time of writing, the upstream repository does not publish an explicit `LICENSE` file. Because of that, this fork should be treated as a maintained fork of the original project, not as a newly relicensed codebase.

#### Project Prerequisites
- [.NET 8.0](https://dotnet.microsoft.com/download/dotnet)
- [Battle.net](https://www.blizzard.com/en-us/apps/battle.net/desktop) must be installed, up to date and have been recently signed in to.

#### Arguments
| Argument | Description |
| ------- | :---- |
| --prod | TACT Product **(Required)** |
| --lang | Game/Asset language **(Required)** |
| --dir | Installation Directory **(Required)** |
| --uid | Agent UID (Required if different to the TACT Product) |
| --repair | Repairs the installation opposed to installing/updating it |
| --verbose | Enables/disables verbose progress reporting |
| --post-download | Specifies a file or app to run on completion |
| --help | Shows this table |

- All TACT Products and Agent UIDs can be found [here](https://wowdev.wiki/TACT#Products) however only (green) Active products will work.  
- Languages are listed in [BNetInstaller/Options.cs](BNetInstaller/Options.cs), availability will vary between products.
- If the target directory already contains the provided TACT Product, and an update is available, the product will be updated.

#### Command Line Usage

Example for StarCraft 2, which has a TACT Product of `s2` and an Agent UID of `s2(_locale)`:  

`.\bnetinstaller.exe --prod s2 --uid s2_enus --lang enus --dir "C:\Test"`

#### Error Messages and Codes

Since this tool interacts directly with the Battle.net agent there are no human-readable errors just error codes. This tool does it's best to indicate the potential problem and displays the full exception when it occurs. Additional details can be found in the agent log files which are located within the `%programdata%\Battle.net\Agent\Agent.xxxx` directories.

 Some known and common errors can be found below:

- "Unable to find Agent.exe", the Battle.net app is not installed on your system.
- "Unable to start Agent.exe", the app cannot start the Battle.net agent as Battle.net is either missing or not setup. 
- "Unable to authenticate", the Battle.net app is not signed in to and therefore cannot install games.
- "Unable to install", your computer doesn't meet the minimum specs and/or space requirements.
- "2310", the Battle.net Agent rejected the install request for the selected folder. If this happens, open Battle.net, use `Locate the game` for that folder once, make sure `.product.db` exists there, then retry.
- "2221", the supplied TACT Product is unavailable or invalid.
- "2421", your computer doesn't meet the minimum specs and/or space requirements.
- "3001", the supplied TACT Product requires an encryption key which is missing.

