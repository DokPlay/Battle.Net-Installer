using System.Runtime.CompilerServices;
using BNetInstaller.Endpoints;

namespace BNetInstaller.Operations;

internal abstract class AgentTask<T>(Options options)
{
    private const int MissingProgressLimit = 300;
    private const int ProgressPollDelayMs = 2000;

    private readonly Options _options = options;
    private TaskAwaiter<T>? _awaiter;

    public TaskAwaiter<T> GetAwaiter() => _awaiter ??= InnerTask().GetAwaiter();

    public T GetResult() => GetAwaiter().GetResult();

    protected abstract Task<T> InnerTask();

    protected async Task<bool> PrintProgress(params ProductEndpoint[] endpoints)
    {
        endpoints = endpoints.Where(endpoint => endpoint != null).ToArray();

        if (endpoints.Length == 0)
            return false;

        var locale = _options.Locale.ToString().ToLowerInvariant();
        var statusDirectory = _options.StatusDirectory ?? _options.Directory;
        var cursor = (Left: 0, Top: 0);
        var rewriteProgress = _options.Verbose && TryGetCursorPosition(out cursor);

        static bool TryGetCursorPosition(out (int Left, int Top) position)
        {
            try
            {
                position = Console.GetCursorPosition();
                return true;
            }
            catch
            {
                position = default;
                return false;
            }
        }

        static bool TrySetCursorPosition((int Left, int Top) position)
        {
            try
            {
                Console.SetCursorPosition(position.Left, position.Top);
                return true;
            }
            catch
            {
                return false;
            }
        }

        static void Print(string label, object value) =>
            Console.WriteLine("{0,-20}{1,-70}", label, value);

        static string GetString(JsonNode node, params string[] keys)
        {
            foreach (var key in keys)
            {
                var value = node[key]?.GetValue<string>();

                if (!string.IsNullOrWhiteSpace(value))
                    return value;
            }

            return null;
        }

        static string NormaliseDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return null;

            var normalised = path.Replace('/', '\\').Trim();

            try
            {
                normalised = Path.GetFullPath(normalised);
            }
            catch
            {
                // Agent can return paths that are not valid on this host.
            }

            return normalised.TrimEnd('\\');
        }

        void ResetProgressCursor()
        {
            if (rewriteProgress && !TryGetCursorPosition(out cursor))
                rewriteProgress = false;
        }

        var directoryNoticePrinted = !string.Equals(
            NormaliseDirectory(statusDirectory),
            NormaliseDirectory(_options.Directory),
            StringComparison.OrdinalIgnoreCase);

        void PrintDirectoryNotice(JsonNode stats)
        {
            var agentDirectory = GetString(stats, "install_dir", "game_dir", "directory");
            var agentNormalised = NormaliseDirectory(agentDirectory);
            var requestedNormalised = NormaliseDirectory(_options.Directory);

            if (string.IsNullOrWhiteSpace(agentNormalised) || string.IsNullOrWhiteSpace(requestedNormalised))
                return;

            statusDirectory = agentDirectory;

            if (directoryNoticePrinted)
            {
                ResetProgressCursor();
                return;
            }

            directoryNoticePrinted = true;

            if (string.Equals(agentNormalised, requestedNormalised, StringComparison.OrdinalIgnoreCase))
            {
                ResetProgressCursor();
                return;
            }

            Console.WriteLine();
            Console.WriteLine("Warning: you selected a different install folder, but Battle.net is using an existing Diablo IV registration.");
            Console.WriteLine($"Selected folder:   {_options.Directory}");
            Console.WriteLine($"Battle.net folder: {agentDirectory}");
            Console.WriteLine();
            Console.WriteLine("The download will continue in the Battle.net folder above. To install on the selected drive instead, close this console, move the existing Diablo IV folder there or delete the old folder, then start this installer again.");
            ResetProgressCursor();
        }

        void PrintStatus(string status)
        {
            if (rewriteProgress)
            {
                rewriteProgress = TrySetCursorPosition(cursor);
            }

            if (rewriteProgress)
            {
                Print("Downloading:", _options.Product);
                Print("Language:", locale);
                Print("Directory:", statusDirectory);
                Print("Status:", status);
            }
            else
            {
                Console.WriteLine(status);
            }
        }

        var missingProgressCount = 0;
        var preparingPrinted = false;

        while (true)
        {
            float? progress = null;
            bool? playable = null;

            foreach (var endpoint in endpoints)
            {
                var stats = await endpoint.Get();

                PrintDirectoryNotice(stats);

                var complete = stats["download_complete"]?.GetValue<bool?>();

                if (complete == true)
                    return true;

                progress = stats["progress"]?.GetValue<float?>();

                if (progress.HasValue)
                {
                    playable = stats["playable"]?.GetValue<bool?>();
                    break;
                }
            }

            // get progress percentage and playability
            if (!progress.HasValue)
            {
                missingProgressCount++;

                if (missingProgressCount >= MissingProgressLimit)
                    return false;

                if (!preparingPrinted)
                {
                    PrintStatus("Waiting for Battle.net Agent progress...");
                    preparingPrinted = true;
                }

                await Task.Delay(ProgressPollDelayMs);
                continue;
            }

            missingProgressCount = 0;
            preparingPrinted = false;

            // some non-console environments don't support
            // cursor positioning or line rewriting
            if (rewriteProgress)
            {
                rewriteProgress = TrySetCursorPosition(cursor);
            }

            if (rewriteProgress)
            {
                Print("Downloading:", _options.Product);
                Print("Language:", locale);
                Print("Directory:", statusDirectory);
                Print("Progress:", progress.Value.ToString("P4"));
                Print("Playable:", playable.GetValueOrDefault());
            }
            else
            {
                Print("Progress:", progress.Value.ToString("P4"));
            }

            await Task.Delay(ProgressPollDelayMs);

            // exit @ 100%
            if (progress >= 1f)
                return true;
        }
    }
}
