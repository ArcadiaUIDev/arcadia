using System.Diagnostics;
using System.Net;

namespace Arcadia.Tests.E2E.Infrastructure;

[SetUpFixture]
public class ServerFixture
{
    private Process? _serverProcess;

    [OneTimeSetUp]
    public async Task StartServer()
    {
        // Check if server is already running (e.g. in CI or manual start)
        if (await IsServerRunning())
        {
            Console.WriteLine("Demo server already running at " + TestConstants.BaseUrl);
            return;
        }

        Console.WriteLine("Starting demo server...");
        var projectPath = FindProjectPath();

        _serverProcess = new Process
        {
            StartInfo = new ProcessStartInfo
            {
                FileName = "dotnet",
                Arguments = $"run --project \"{projectPath}\" --framework net9.0 --no-launch-profile",
                Environment =
                {
                    ["ASPNETCORE_URLS"] = TestConstants.BaseUrl,
                    ["ASPNETCORE_ENVIRONMENT"] = "Development"
                },
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            }
        };

        _serverProcess.Start();

        // Wait for server to be ready (max 60 seconds)
        var timeout = TimeSpan.FromSeconds(60);
        var sw = Stopwatch.StartNew();
        while (sw.Elapsed < timeout)
        {
            if (await IsServerRunning())
            {
                Console.WriteLine($"Demo server ready after {sw.Elapsed.TotalSeconds:F1}s");
                return;
            }
            await Task.Delay(1000);
        }

        throw new TimeoutException($"Demo server did not start within {timeout.TotalSeconds}s");
    }

    [OneTimeTearDown]
    public void StopServer()
    {
        if (_serverProcess is not null && !_serverProcess.HasExited)
        {
            _serverProcess.Kill(entireProcessTree: true);
            _serverProcess.Dispose();
            Console.WriteLine("Demo server stopped.");
        }
    }

    private static async Task<bool> IsServerRunning()
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(3) };
            var response = await client.GetAsync(TestConstants.BaseUrl);
            return response.StatusCode == HttpStatusCode.OK;
        }
        catch
        {
            return false;
        }
    }

    private static string FindProjectPath()
    {
        // Walk up from test assembly to find the repo root
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "samples", "Arcadia.Demo.Server", "Arcadia.Demo.Server.csproj");
            if (File.Exists(candidate)) return candidate;
            dir = dir.Parent;
        }
        throw new FileNotFoundException("Could not find Arcadia.Demo.Server.csproj. Run tests from the repo root.");
    }
}
