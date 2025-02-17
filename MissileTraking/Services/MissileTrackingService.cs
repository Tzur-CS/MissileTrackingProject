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
            var missileInterceptor = new MissileInterceptor(_createDbContext, policy);
            _commandFactory = new CommandFactory(missileInterceptor);
        }

        public async Task ProcessCommandAsync(NetworkStream stream)
        {
            byte[] buffer = new byte[1024];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            string  receivedData =  Encoding.UTF8.GetString(buffer, 0, bytesRead);

            Dictionary<string, string> map = Parser.Parse(receivedData);
            
            string commandType = map["CommandType"];
            string? requestData = map.ContainsKey("args") ? map["args"] : null;
            Console.WriteLine($"[Server] Received: {receivedData}");
            
            await ExecuteCommandAsync(commandType, requestData, stream);
        }

        public async Task ExecuteCommandAsync(string commandType, string? requestData, NetworkStream stream)
        {
            ICommand command = _commandFactory.GetCommand(commandType);
            if (command != null)
            {
                await command.ExecuteAsync(requestData, stream, _createDbContext);
            }
            else
            {
                await SendResponseAsync(stream, "Unknown command");
            }
        }

        private async Task SendResponseAsync(NetworkStream stream, string message)
        {
            byte[] responseData = System.Text.Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(responseData, 0, responseData.Length);
        }
    }
}