using MissileTracking.Interception;

namespace MissileTracking.Commands
{
    public class CommandFactory
    {
        private readonly Dictionary<string, Func<ICommand>> _commands;

        
        public CommandFactory(MissileInterceptor missileInterceptor)
        {
            _commands = new Dictionary<string, Func<ICommand>>
            {
                { "MissileInfo", () => new AddMissileCommand(missileInterceptor) },
                { "GetMissilesByCity", () => new GetMissilesByCityCommand() },
                { "GetMissileStats", () => new GetMissileStatsCommand() },
                { "GenerateReport", () => new GenerateMissileReportCommand() }
            };
        }

        public ICommand GetCommand(string commandType)
        {
            return _commands.ContainsKey(commandType) ? _commands[commandType]() : null;
        }
    }
}