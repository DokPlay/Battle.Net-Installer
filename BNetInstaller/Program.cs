using System.Diagnostics;
using System.Globalization;
using BNetInstaller.Operations;

namespace BNetInstaller;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        if (args is not { Length: > 0 })
            args = OptionsBinder.CreateArgs();

        await OptionsBinder
            .BuildRootCommand(Run)
            .Parse(args)
            .InvokeAsync();
    }

    private static async Task Run(Options options)
    {
        using AgentApp app = new();
        options.Sanitise();
        var mode = options.Repair ? Mode.Repair : Mode.Install;

        // check if target directory exists before requesting bnet agent to validate
        Directory.CreateDirectory(options.Directory);
        InstallDiagnostics.PrintPreflightWarnings(options.Directory, mode == Mode.Repair);

        if (options.Verbose)
        {
            try
            {
                var root = Path.GetPathRoot(options.Directory);
                if (!string.IsNullOrWhiteSpace(root))
                {
                    var drive = new DriveInfo(root);
                    var gib = drive.AvailableFreeSpace / (1024d * 1024d * 1024d);
                    Console.WriteLine($"Target drive free space: {gib.ToString("F2", CultureInfo.InvariantCulture)} GiB");
                }
            }
            catch
            {
            }
        }

        var localeCode = options.Locale.ToString().ToLowerInvariant();

        Console.WriteLine("Authenticating");
        await app.AgentEndpoint.Get();

        try
        {
            Console.WriteLine($"Queuing {mode}");
            app.InstallEndpoint.Model.InstructionsPatchUrl = $"http://us.patch.battle.net:1119/{options.Product}";
            app.InstallEndpoint.Model.Uid = options.UID;
            await app.InstallEndpoint.Post();

            Console.WriteLine($"Starting {mode}");
            app.InstallEndpoint.Product.Model.GameDir = options.Directory;
            app.InstallEndpoint.Product.Model.Language = [localeCode];
            app.InstallEndpoint.Product.Model.SelectedLocale = localeCode;
            app.InstallEndpoint.Product.Model.SelectedAssetLocale = localeCode;
            await app.InstallEndpoint.Product.Post();
        }
        catch (AgentException ex) when (ex.ErrorCode == 2310)
        {
            InstallDiagnostics.PrintAgentTroubleshooting(ex.ErrorCode);
            await app.AgentEndpoint.Delete();
            return;
        }

        Console.WriteLine();

        AgentTask<bool> operation = mode switch
        {
            Mode.Install => new InstallProductTask(options, app),
            Mode.Repair => new RepairProductTask(options, app),
            _ => throw new NotSupportedException(),
        };

        // process the task
        var complete = await operation;

        // send close signal
        await app.AgentEndpoint.Delete();

        // run the post download app/script if applicable
        if (complete && !string.IsNullOrWhiteSpace(options.PostDownload) && File.Exists(options.PostDownload))
            Process.Start(options.PostDownload);
    }
}

file enum Mode
{
    Install,
    Repair
}
