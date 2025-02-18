using System.Text;
using Microsoft.EntityFrameworkCore;
using MissileTracking.Database;
using MissileTracking.Services;

namespace MissileTracking
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            var options = DbSetUp();

            var dbContextProvider = () => new MissileDbContext(options);
            var trackingService = new MissileTrackingService(dbContextProvider, ["CityA", "CityB"]);

            var server = new TcpConnectionService(trackingService);

            await server.StartAsync();
        }

        private static DbContextOptions<MissileDbContext> DbSetUp()
        {
            var options = new DbContextOptionsBuilder<MissileDbContext>()
                .UseSqlite("Data Source=missiles.db")
                .Options;

            if (File.Exists("missiles.db"))
            {
                File.Delete("missiles.db");
            }

            using var context = new MissileDbContext(options);
            context.Database.EnsureCreated();

            return options;
        }
    }
}