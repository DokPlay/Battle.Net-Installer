namespace BNetInstaller;

internal static class InstallDiagnostics
{
    public static string PrintPreflightWarnings(string directory, string product, bool repair)
    {
        if (repair)
            return null;

        var productDbPath = Path.Combine(directory, ".product.db");

        if (File.Exists(productDbPath))
            return null;

        var registeredDirectory = FindRegisteredInstallDirectory(directory, product);

        if (registeredDirectory == null)
            return null;

        Console.WriteLine("Notice: you selected a different install folder, but Diablo IV is already registered elsewhere.");
        Console.WriteLine($"Selected folder: {directory}");
        Console.WriteLine($"Registered folder: {registeredDirectory}");
        Console.WriteLine("Battle.net will continue the download using the registered folder above.");
        Console.WriteLine("To install Diablo IV on the selected drive instead, close this console, move the existing Diablo IV folder to the selected folder or delete the old folder, then start this installer again.");
        Console.WriteLine();

        return registeredDirectory;
    }

    private static string FindRegisteredInstallDirectory(string selectedDirectory, string product)
    {
        var selected = NormaliseDirectory(selectedDirectory);

        foreach (var candidate in GetInstallDirectoryCandidates(selectedDirectory))
        {
            var candidatePath = NormaliseDirectory(candidate);

            if (string.IsNullOrWhiteSpace(candidatePath))
                continue;

            if (string.Equals(candidatePath, selected, StringComparison.OrdinalIgnoreCase))
                continue;

            var productDbPath = Path.Combine(candidatePath, ".product.db");

            if (!File.Exists(productDbPath))
                continue;

            if (!ProductDbMatches(productDbPath, product))
                continue;

            return candidatePath;
        }

        return null;
    }

    private static IEnumerable<string> GetInstallDirectoryCandidates(string selectedDirectory)
    {
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var candidate in GetLikelyInstallDirectoryCandidates(selectedDirectory))
        {
            if (!string.IsNullOrWhiteSpace(candidate) && seen.Add(candidate))
                yield return candidate;
        }
    }

    private static IEnumerable<string> GetLikelyInstallDirectoryCandidates(string selectedDirectory)
    {
        var selectedName = Path.GetFileName(NormaliseDirectory(selectedDirectory));

        foreach (var drive in DriveInfo.GetDrives())
        {
            if (!drive.IsReady)
                continue;

            if (!string.IsNullOrWhiteSpace(selectedName))
                yield return Path.Combine(drive.RootDirectory.FullName, selectedName);

            yield return Path.Combine(drive.RootDirectory.FullName, "Diablo IV");
            yield return Path.Combine(drive.RootDirectory.FullName, "Games", "Diablo IV");
            yield return Path.Combine(drive.RootDirectory.FullName, "Battle.net", "Diablo IV");

            IEnumerable<DirectoryInfo> rootDirectories;

            try
            {
                rootDirectories = drive.RootDirectory.EnumerateDirectories("*Diablo*", SearchOption.TopDirectoryOnly);
            }
            catch
            {
                continue;
            }

            foreach (var directory in rootDirectories)
                yield return directory.FullName;
        }
    }

    private static bool ProductDbMatches(string productDbPath, string product)
    {
        if (string.IsNullOrWhiteSpace(product))
            return true;

        try
        {
            var content = File.ReadAllText(productDbPath);
            return content.Contains(product, StringComparison.OrdinalIgnoreCase);
        }
        catch
        {
            return false;
        }
    }

    private static string NormaliseDirectory(string directory)
    {
        if (string.IsNullOrWhiteSpace(directory))
            return null;

        try
        {
            return Path.GetFullPath(directory.Replace('/', '\\').Trim()).TrimEnd('\\');
        }
        catch
        {
            return directory.Replace('/', '\\').Trim().TrimEnd('\\');
        }
    }

    public static void PrintAgentTroubleshooting(int errorCode)
    {
        switch (errorCode)
        {
            case 2310:
                Console.WriteLine("Agent returned 2310 while preparing the install.");
                Console.WriteLine("Battle.net rejected the selected install folder.");
                Console.WriteLine("If Diablo IV is already registered elsewhere, close this console, move that folder to the selected drive or delete the old folder, then start this installer again.");
                break;
            case 2421:
                Console.WriteLine("Agent returned 2421 (minimum specs and/or disk space requirement not met).");
                Console.WriteLine("Double-check the install directory/drive has enough free space and that the product meets OS/hardware requirements.");
                break;
        }
    }
}
