namespace BNetInstaller;

internal static class InstallDiagnostics
{
    public static void PrintPreflightWarnings(string directory, bool repair)
    {
        if (repair)
            return;

        var productDbPath = Path.Combine(directory, ".product.db");

        if (File.Exists(productDbPath))
            return;

        Console.WriteLine("Warning: .product.db was not found in the target directory.");
        Console.WriteLine("If Battle.net returns Error 2310, open the Battle.net client, use 'Locate the game' for this folder once, then retry.");
        Console.WriteLine();
    }

    public static void PrintAgentTroubleshooting(int errorCode)
    {
        switch (errorCode)
        {
            case 2310:
                Console.WriteLine("Agent returned 2310 while preparing the install.");
                Console.WriteLine("This usually means Battle.net rejected the install metadata for the selected folder.");
                Console.WriteLine("Try opening Battle.net, using 'Locate the game' for this folder, then retrying once .product.db exists.");
                break;
            case 2421:
                Console.WriteLine("Agent returned 2421 (minimum specs and/or disk space requirement not met).");
                Console.WriteLine("Double-check the install directory/drive has enough free space and that the product meets OS/hardware requirements.");
                break;
        }
    }
}
