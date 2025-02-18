using System.Net.Sockets;
using System.Text;
using MissileTracking.Database;
using MissileTracking.Services;

namespace MissileTracking.Commands;

public class ChangePolicyCommand : ICommand
{
    private readonly HashSet<string> _policy;
    private readonly object _policyLock = new(); // Thread safety

    public ChangePolicyCommand(HashSet<string> policy)
    {
        _policy = policy;
    }

    public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
    {
        Console.WriteLine($"[Policy Change] Received request: {request}");

        if (string.IsNullOrWhiteSpace(request))
        {
            await TcpConnectionService.SendResponseAsync(stream,
                "❌ Invalid format. Use: 'ChangePolicy@add@CityA' or 'ChangePolicy@remove@CityA'");
            return;
        }

        var parts = request.Split('@', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
        {
            await TcpConnectionService.SendResponseAsync(stream,
                "❌ Invalid format. Use: 'ChangePolicy@add@CityA' or 'ChangePolicy@remove@CityA'");
            return;
        }

        string command = parts[0].Trim();
        string location = parts[1].Trim();

        string result = ChangePolicy(command, location);
        Console.WriteLine($"[Policy Change] {result}");
        await TcpConnectionService.SendResponseAsync(stream, result);
    }

    private string ChangePolicy(string command, string location)
    {
        lock (_policyLock) // Thread safety
        {
            if (command.Equals("add", StringComparison.OrdinalIgnoreCase))
            {
                return _policy.Add(location)
                    ? $"✅ Location '{location}' added to interception policy."
                    : $"⚠ Location '{location}' is already in the interception policy.";
            }

            if (!command.Equals("remove", StringComparison.OrdinalIgnoreCase))
                return $"❌ Invalid command '{command}'. Use 'add' or 'remove'.";
            return _policy.Remove(location)
                ? $"❌ Location '{location}' removed from interception policy."
                : $"⚠ Location '{location}' was not in the interception policy.";
        }
    }
}