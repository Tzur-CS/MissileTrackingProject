using System.Net.Sockets;
using MissileTracking.Database;
using MissileTracking.Models;
using MissileTracking.Services;

namespace MissileTracking.Commands
{
    public class GetMissilesByCityCommand : ICommand
    {
        public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        {
            Console.WriteLine("Getting missiles by City");

            List<MissileInfo> missiles;

            // Ensure the database context is used safely
            await using (var context = dbContextProvider())
            {
                var repository = new MissileRepository(context);
                missiles = await repository.GetMissilesByHitLocationAsync(request); 
            }

            //  Ensure we process the results AFTER the DbContext is closed
            if (missiles == null || missiles.Count == 0)
            {
                Console.WriteLine($"[Warning] No missiles found for location: {request}");
                missiles = new List<MissileInfo>(); // Ensure it's never null
            }

            var response = string.Join(";\n", missiles.Select(m =>
                $"{m.Type}, HitLocation: {m.HitLocation}, TryToIntercepted: {m.IsIntercepted}, IsInterceptSuccess: {m.InterceptSuccess}"));

            if (string.IsNullOrWhiteSpace(response))
            {
                response = "No missiles found for the given location.";
            }

            await TcpConnectionService.SendResponseAsync(stream, response);
        }
        
    }
}