using System.Net.Sockets;
using MissileTracking.Commands;
using MissileTracking.Database;
using MissileTracking.Interception;
using System.Text;
using MissileTracking.Parsers;

namespace MissileTracking.Services
{
    public class MissileTrackingService
    {
        private readonly Func<MissileDbContext> _createDbContext;
        private readonly CommandFactory _commandFactory;

        public MissileTrackingService(Func<MissileDbContext> createDbContext, HashSet<string> policy)
        {
            _createDbContext = createDbContext;
            var missileInterceptor = new MissileInterceptorLogic(_createDbContext, policy);
            _commandFactory = new CommandFactory(missileInterceptor);
        }

        public async Task ProcessCommandAsync(NetworkStream stream)
        {
            var buffer = new byte[1024];
            var bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            var  receivedData =  Encoding.UTF8.GetString(buffer, 0, bytesRead);
            var commandMap = Parser.Parse(receivedData);
            
            var commandType = commandMap["CommandType"];
            var requestData = commandMap.TryGetValue("args", out var value) ? value : null;
            Console.WriteLine($"[Server] Received: {receivedData}");
            
            await ExecuteCommandAsync(commandType, requestData, stream);
        }

        private async Task ExecuteCommandAsync(string commandType, string? requestData, NetworkStream stream)
        {
            var command = _commandFactory.GetCommand(commandType);
            if (command != null)
            {
                await command.ExecuteAsync(requestData, stream, _createDbContext);
            }
            else
            {
                await SendResponseAsync(stream, "Unknown command");
            }
        }

        private static async Task SendResponseAsync(NetworkStream stream, string message)
        {
            var responseData = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(responseData, 0, responseData.Length);
        }
    }
}