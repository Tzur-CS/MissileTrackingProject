using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using MissileTracking.Database;
using MissileTracking.Services;

namespace MissileTracking.Commands
{
    public class GetMissileStatsCommand : ICommand
    {
        public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        {
            await using (var context = dbContextProvider())
            {
                var totalMissiles = context.Missiles.Count();
                var interceptedMissiles = context.Missiles.Count(m => m.IsIntercepted);
                var successfulIntercepts = context.Missiles.Count(m => m.InterceptSuccess);

                var report = $"Total Missiles: {totalMissiles}\n" +
                             $"Intercepted: {interceptedMissiles}\n" +
                             $"Successful Intercepts: {successfulIntercepts}\n" +
                             $"Success Rate: {(interceptedMissiles > 0 ? ((double)successfulIntercepts / interceptedMissiles * 100).ToString("F2") : "0")} %";
                
                await TcpConnectionService.SendResponseAsync(stream, report);
            }
        }
    }
}