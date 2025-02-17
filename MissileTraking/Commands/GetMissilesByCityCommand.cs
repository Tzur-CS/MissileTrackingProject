using System.Text;
using System.Net.Sockets;
using MissileTracking.Database;

namespace MissileTracking.Commands
{
    public class GetMissilesByCityCommand : ICommand
    {
        public async Task ExecuteAsync(string request, NetworkStream stream, Func<MissileDbContext> dbContextProvider)
        {
            Console.WriteLine("Getting missiles by City");
            using (var context = dbContextProvider())
            {
                var repository = new MissileRepository(context);
                var missiles = repository.GetMissilesByHitLocation(request);

                string response = string.Join(";\n", missiles.Select(m => $"{m.Type},HitLocation:{m.HitLocation}, TryToIntercepted:{m.IsIntercepted}, IsInterceptSuccess:{m.InterceptSuccess}"));
                byte[] responseData = Encoding.UTF8.GetBytes(response);
                await stream.WriteAsync(responseData, 0, responseData.Length);
            }
        }
    }
}