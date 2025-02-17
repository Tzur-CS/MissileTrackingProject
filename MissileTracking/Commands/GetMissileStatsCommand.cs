using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using MissileTracking.Database;

namespace MissileTracking.Commands
{
    public class GetMissileStatsCommand : ICommand
    {
        public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        {
            using (var context = dbContextProvider())
            {
                int totalMissiles = context.Missiles.Count();
                int interceptedMissiles = context.Missiles.Count(m => m.IsIntercepted);
                int successfulIntercepts = context.Missiles.Count(m => m.InterceptSuccess);

                string report = $"Total Missiles: {totalMissiles}\n" +
                                $"Intercepted: {interceptedMissiles}\n" +
                                $"Successful Intercepts: {successfulIntercepts}\n" +
                                $"Success Rate: {(interceptedMissiles > 0 ? ((double)successfulIntercepts / interceptedMissiles * 100).ToString("F2") : "0")} %";

                byte[] responseData = Encoding.UTF8.GetBytes(report);
                await stream.WriteAsync(responseData, 0, responseData.Length);
            }
        }
    }
}