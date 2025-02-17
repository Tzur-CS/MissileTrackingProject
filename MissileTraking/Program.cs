using Microsoft.EntityFrameworkCore;
using MissileTracking.Database;
using MissileTracking.Services;

namespace MissileTracking
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<MissileDbContext>()
                .UseSqlite("Data Source=missiles.db")
                .Options;

            if (File.Exists("missiles.db"))
            {
                File.Delete("missiles.db");
            }

            using (var context = new MissileDbContext(options))
            {
                context.Database.EnsureCreated();
            }

            Func<MissileDbContext> dbContextProvider = () => new MissileDbContext(options);
            HashSet<string> interceptionPolicy = new HashSet<string> { "CityA", "CityB" };

            var trackingService = new MissileTrackingService(dbContextProvider, interceptionPolicy);
            var server = new TcpConnectionService(5000, trackingService);

            await server.StartAsync();
        }
    }
}