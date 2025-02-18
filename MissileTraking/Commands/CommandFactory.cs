using MissileTracking.Interception;

namespace MissileTracking.Commands;

public class CommandFactory
{
    private readonly Dictionary<string, Func<ICommand>> _commands;

    public CommandFactory(MissileInterceptorLogic missileInterceptor, HashSet<string> policy)
    {
        _commands = new Dictionary<string, Func<ICommand>>
        {
            { "MissileInfo", () => new AddMissileCommand(missileInterceptor) },
            { "GetMissilesByCity", () => new GetMissilesByCityCommand() },
            { "GetMissileStats", () => new GetMissileStatsCommand() },
            { "GenerateReport", () => new GenerateMissileReportCommand() },
            { "ChangePolicy", () => new ChangePolicyCommand(policy) } 
        };
    }

    public ICommand? GetCommand(string commandType)
    {
        return _commands.TryGetValue(commandType, out var value) ? value() : null;
    }
}