using System.Net.Sockets;
using MissileTracking.Database;
using MissileTracking.Models;
using MissileTracking.Interception;
using MissileTracking.Services;

namespace MissileTracking.Commands
{
    public class AddMissileCommand : ICommand
    {
        private readonly MissileInterceptorLogic _missileInterceptor;

        public AddMissileCommand(MissileInterceptorLogic missileInterceptor)
        {
            _missileInterceptor = missileInterceptor;
        }

        public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        {
            var parts = request.Split(',');
            if (parts.Length != 4 || !int.TryParse(parts[1], out _) || !int.TryParse(parts[2], out _))
            {
                await TcpConnectionService.SendResponseAsync(stream, "Invalid missile data format");
                return;
            }
            
            var missile = new MissileInfo
            {
                Type = parts[0],
                X = int.Parse(parts[1]),
                Y = int.Parse(parts[2]),
                HitLocation = parts[3]
            };

            using (var context = dbContextProvider())
            {
                var repository = new MissileRepository(context);

                // Subscribe to the event
                repository.MissileAdded += (sender, args) =>
                {
                    Console.WriteLine(
                        $"[Event] New missile added: {args.Missile.Type} at ({args.Missile.X}, {args.Missile.Y})");
                };

                await repository.AddMissileAsync(missile);
            }

            Console.WriteLine("[Server] Missile stored successfully.");
            _ = _missileInterceptor.AttemptInterceptAsync(missile);

            await TcpConnectionService.SendResponseAsync(stream, "Missile added successfully");
        }
    }
}
