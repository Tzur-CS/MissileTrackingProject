using System.Text;
using System.Net.Sockets;
using MissileTracking.Database;
using MissileTracking.Models;
using MissileTracking.Interception;

namespace MissileTracking.Commands
{
    public class AddMissileCommand : ICommand
    {
        private readonly MissileInterceptor _missileInterceptor;

        public AddMissileCommand(MissileInterceptor missileInterceptor)
        {
            _missileInterceptor = missileInterceptor;
        }

        // public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        // {
        //     var parts = request.Split(',');
        //
        //     if (parts.Length < 4)
        //     {
        //         byte[] errorResponse = Encoding.UTF8.GetBytes("Invalid missile data format");
        //         await stream.WriteAsync(errorResponse, 0, errorResponse.Length);
        //         return;
        //     }
        //
        //     var missile = new MissileInfo
        //     {
        //         Type = parts[0],
        //         X = int.Parse(parts[1]),
        //         Y = int.Parse(parts[2]),
        //         HitLocation = parts[3]
        //     };
        //
        //     using (var context = dbContextProvider())
        //     {
        //         var repository = new MissileRepository(context);
        //         await repository.AddMissileAsync(missile);
        //     }
        //
        //     Console.WriteLine("[Server] Missile stored successfully.");
        //     _ = _missileInterceptor.AttemptInterceptAsync(missile);
        //
        //     byte[] response = Encoding.UTF8.GetBytes("Missile added successfully");
        //     await stream.WriteAsync(response, 0, response.Length);
        // }
        
        public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        {
            var parts = request.Split(',');

            if (parts.Length < 4)
            {
                byte[] errorResponse = Encoding.UTF8.GetBytes("Invalid missile data format");
                await stream.WriteAsync(errorResponse, 0, errorResponse.Length);
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
                    Console.WriteLine($"[Event] New missile added: {args.Missile.Type} at ({args.Missile.X}, {args.Missile.Y})");
                };

                await repository.AddMissileAsync(missile);
            }

            Console.WriteLine("[Server] Missile stored successfully.");
            _ = _missileInterceptor.AttemptInterceptAsync(missile);

            byte[] response = Encoding.UTF8.GetBytes("Missile added successfully");
            await stream.WriteAsync(response, 0, response.Length);
        }

    }
}