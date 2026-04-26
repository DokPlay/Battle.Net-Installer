using System.Diagnostics;
using System.Globalization;
using BNetInstaller.Operations;

namespace BNetInstaller;

internal static class Program
{
    private static async Task Main(string[] args)
    {
        var interactive = args is not { Length: > 0 };

        if (interactive)
            args = OptionsBinder.CreateArgs();

        try
        {
            await OptionsBinder
                .BuildRootCommand(Run)
                .Parse(args)
                .InvokeAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            Environment.ExitCode = 1;
        }
        finally
        {
            if (interactive && Environment.ExitCode != 0)
            {
                Console.WriteLine();
                Console.Write("Press Enter to exit...");
                Console.ReadLine();
            }
        }
    }

    private static async Task Run(Options options)
    {
        options.Sanitise();
        var mode = options.Repair ? Mode.Repair : Mode.Install;

        if (mode == Mode.Repair)
        {
            if (!Directory.Exists(options.Directory))
            {
                Console.WriteLine($"Unable to repair because the target directory does not exist: {options.Directory}");
                return;
            }
        }
        else
        {
            // check if target directory exists before requesting bnet agent to validate
            Directory.CreateDirectory(options.Directory);
            options.StatusDirectory = InstallDiagnostics.PrintPreflightWarnings(options.Directory, options.Product, repair: false) ?? options.Directory;
        }

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
        var complete = false;
        var authenticated = false;

        using AgentApp app = new();

        try
        {
            Console.WriteLine("Authenticating");
            await app.AgentEndpoint.Get();
            authenticated = true;

            Console.WriteLine($"Queuing {mode}");
            app.InstallEndpoint.Model.InstructionsPatchUrl = $"http://us.patch.battle.net:1119/{options.Product}";
            app.InstallEndpoint.Model.Uid = options.UID;
            await app.InstallEndpoint.Post();

            Console.WriteLine($"Starting {mode}");
            if (app.InstallEndpoint.Product == null)
                throw new AgentException(0, "Agent Error: response_uri was missing from the install response.");

            app.InstallEndpoint.Product.Model.GameDir = options.Directory;
            app.InstallEndpoint.Product.Model.Language = [localeCode];
            app.InstallEndpoint.Product.Model.SelectedLocale = localeCode;
            app.InstallEndpoint.Product.Model.SelectedAssetLocale = localeCode;
            await app.InstallEndpoint.Product.Post();

            Console.WriteLine();

            AgentTask<bool> operation = mode switch
            {
                Mode.Install => new InstallProductTask(options, app),
                Mode.Repair => new RepairProductTask(options, app),
                _ => throw new NotSupportedException(),
            };

            // process the task
            complete = await operation;

            if (!complete)
                Environment.ExitCode = 1;
        }
        catch (AgentException ex) when (ex.ErrorCode is 2310 or 2421)
        {
            InstallDiagnostics.PrintAgentTroubleshooting(ex.ErrorCode);
            Environment.ExitCode = 1;
        }
        catch (AgentException ex)
        {
            Console.WriteLine(ex.Message);
            Environment.ExitCode = 1;
        }
        finally
        {
            if (authenticated)
            {
                try
                {
                    await app.AgentEndpoint.Delete();
                }
                catch (Exception ex)
                {
                    if (options.Verbose)
                        Console.WriteLine($"Unable to notify Agent to close: {ex.Message}");
                }
            }
        }

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
