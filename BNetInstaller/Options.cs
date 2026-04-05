using System.CommandLine;
using System.Text.RegularExpressions;

namespace BNetInstaller;

internal sealed partial class Options
{
    public string Product { get; set; }
    public Locale Locale { get; set; }
    public string Directory { get; set; }
    public string UID { get; set; }
    public bool Repair { get; set; }
    public bool Verbose { get; set; }
    public string PostDownload { get; set; }

    public void Sanitise()
    {
        // ensure a UID exists
        if (string.IsNullOrWhiteSpace(UID))
            UID = Product;

        // remove _locale suffix for wiki copy-pasters
        if (UID.Contains("_locale", StringComparison.OrdinalIgnoreCase))
            UID = ExtractLocaleRegex().Replace(UID, $"_{Locale}");

        Product = Product.ToLowerInvariant().Trim();
        UID = UID.ToLowerInvariant().Trim();
        Directory = Path.GetFullPath(Directory + "\\");
    }

    [GeneratedRegex("\\(?_locale\\)?", RegexOptions.IgnoreCase)]
    private static partial Regex ExtractLocaleRegex();
}

internal static class OptionsBinder
{
    private const string LocaleExamples = "enUS, ruRU, deDE, frFR";

    private static readonly Option<string> Product = new("--prod")
    {
        HelpName = "TACT Product",
        Required = true
    };

    private static readonly Option<Locale> Locale = new("--lang")
    {
        HelpName = "Game/Asset language",
        Required = true
    };

    private static readonly Option<string> Directory = new("--dir")
    {
        HelpName = "Installation Directory",
        Required = true
    };

    private static readonly Option<string> UID = new("--uid")
    {
        HelpName = "Agent Product UID (Required if different to the TACT product)",
        Required = true
    };

    private static readonly Option<bool> Repair = new("--repair")
    {
        HelpName = "Run installation repair"
    };

    private static readonly Option<bool> Verbose = new("--verbose")
    {
        HelpName = "Enables/disables verbose progress reporting",
        DefaultValueFactory = (_) => true
    };

    private static readonly Option<string> PostDownload = new("--post-download")
    {
        HelpName = "Specifies a file or app to run on completion"
    };

    public static string[] CreateArgs()
    {
        static string GetInput(string message)
        {
            Console.Write(message);
            return Console.ReadLine()?.Trim().Trim('"') ?? string.Empty;
        }

        static string GetRequiredInput(string message)
        {
            while (true)
            {
                var value = GetInput(message);

                if (!string.IsNullOrWhiteSpace(value))
                    return value;

                Console.WriteLine("This value is required.");
            }
        }

        static bool GetYesNo(string message)
        {
            while (true)
            {
                var value = GetInput(message);

                if (string.IsNullOrWhiteSpace(value))
                    return false;

                if (value.StartsWith("Y", StringComparison.OrdinalIgnoreCase))
                    return true;

                if (value.StartsWith("N", StringComparison.OrdinalIgnoreCase))
                    return false;

                Console.WriteLine("Please enter Y or N.");
            }
        }

        Console.WriteLine("Please complete the following information:");
        Console.WriteLine("Press Enter on Agent UID if it should match the TACT Product.");
        Console.WriteLine($"Locale examples: {LocaleExamples}");
        Console.WriteLine();

        var product = GetRequiredInput("TACT Product (example: s2): ");
        var uid = GetInput("Agent UID (example: s2_enUS, blank = same as product): ");
        var directory = GetRequiredInput("Installation Directory (example: D:\\Battle.net\\StarCraft II): ");
        var locale = GetRequiredInput("Game/Asset Language (example: enUS): ");
        var repair = GetYesNo("Repair Install? (Y/N, default N): ");

        var args = new string[9]
        {
            "--prod",
            product,
            "--uid",
            uid,
            "--dir",
            directory,
            "--lang",
            locale,
            repair ? "--repair" : string.Empty
        };

        Console.WriteLine();

        return args;
    }

    public static RootCommand BuildRootCommand(Func<Options, Task> task)
    {
        var rootCommand = new RootCommand()
        {
            Product,
            Locale,
            Directory,
            UID,
            Repair,
            Verbose,
            PostDownload
        };

        rootCommand.SetAction(async context =>
        {
            await task(new()
            {
                Product = context.CommandResult.GetValue(Product),
                Locale = context.CommandResult.GetValue(Locale),
                Directory = context.CommandResult.GetValue(Directory),
                UID = context.CommandResult.GetValue(UID),
                Repair = context.CommandResult.GetValue(Repair),
                Verbose = context.CommandResult.GetValue(Verbose),
                PostDownload = context.CommandResult.GetValue(PostDownload),
            });
        });

        rootCommand.TreatUnmatchedTokensAsErrors = false;

        return rootCommand;
    }
}

internal enum Locale
{
    arSA,
    enSA,
    deDE,
    enUS,
    esMX,
    ptBR,
    esES,
    frFR,
    itIT,
    koKR,
    plPL,
    ruRU,
    zhCN,
    zhTW
}
