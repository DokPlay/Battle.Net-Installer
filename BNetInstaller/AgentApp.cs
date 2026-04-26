using System.ComponentModel;
using System.Diagnostics;
using BNetInstaller.Endpoints.Agent;
using BNetInstaller.Endpoints.Install;
using BNetInstaller.Endpoints.Repair;
using BNetInstaller.Endpoints.Update;
using BNetInstaller.Endpoints.Version;

namespace BNetInstaller;

internal sealed class AgentApp : IDisposable
{
    private static readonly TimeSpan AgentStartTimeout = TimeSpan.FromSeconds(30);

    public readonly AgentEndpoint AgentEndpoint;
    public readonly InstallEndpoint InstallEndpoint;
    public readonly UpdateEndpoint UpdateEndpoint;
    public readonly RepairEndpoint RepairEndpoint;
    public readonly VersionEndpoint VersionEndpoint;

    private readonly Process _process;
    private readonly int _port;
    private readonly AgentClient _client;

    public AgentApp()
    {
        if (!StartProcess(out _process, out _port))
        {
            Console.WriteLine("Please ensure Battle.net is installed and has recently been signed in to.");
            Environment.Exit(1);
        }

        _client = new(_port);

        AgentEndpoint = new(_client);
        InstallEndpoint = new(_client);
        UpdateEndpoint = new(_client);
        RepairEndpoint = new(_client);
        VersionEndpoint = new(_client);
    }

    private static bool StartProcess(out Process process, out int port)
    {
        (process, port) = (null, -1);

        var agentPath = GetAgentPath();

        if (!File.Exists(agentPath))
        {
            Console.WriteLine("Unable to find Agent.exe.");
            return false;
        }

        try
        {
            process = Process.Start(new ProcessStartInfo(agentPath)
            {
                Arguments = "--internalclienttools",
                UseShellExecute = true,
            });

            if (process == null)
            {
                Console.WriteLine("Unable to start Agent.exe.");
                return false;
            }

            var stopwatch = Stopwatch.StartNew();

            // detect listening port
            while (process is { HasExited: false } && port == -1 && stopwatch.Elapsed < AgentStartTimeout)
            {
                Thread.Sleep(250);
                port = NativeMethods.GetProcessListeningPort(process.Id);
            }

            if (process is not { HasExited: false } || port == -1)
            {
                if (port == -1)
                    Console.WriteLine("Timed out waiting for Agent.exe to open its local control port.");
                else
                    Console.WriteLine("Unable to connect to Agent.exe.");

                StopProcess(process);
                return false;
            }

            return true;
        }
        catch (Win32Exception)
        {
            Console.WriteLine("Unable to start Agent.exe.");
            return false;
        }
    }

    private static void StopProcess(Process process)
    {
        try
        {
            if (!process.HasExited)
                process.Kill(entireProcessTree: true);
        }
        catch (InvalidOperationException)
        {
        }
        catch (Win32Exception)
        {
        }
    }

    private static string GetAgentPath()
    {
        var agentDirectory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Battle.net", "Agent");
        var parentPath = Path.Combine(agentDirectory, "Agent.exe");
        var parentVersion = 0;

        // read parent Agent.exe version
        if (File.Exists(parentPath))
            parentVersion = FileVersionInfo.GetVersionInfo(parentPath).ProductPrivatePart;

        // return expected child Agent path
        return Path.Combine(agentDirectory, $"Agent.{parentVersion}", "Agent.exe");
    }

    public void Dispose()
    {
        if (_process?.HasExited == false)
            StopProcess(_process);

        _client?.Dispose();
        _process?.Dispose();
    }
}
