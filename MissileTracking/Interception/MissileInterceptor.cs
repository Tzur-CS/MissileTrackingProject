using MissileTracking.Database;
using MissileTracking.Models;

namespace MissileTracking.Interception
{
    public class MissileInterceptor
    {
        private readonly Func<MissileDbContext> _dbcontextprovider;
        private readonly HashSet<string> _policy;
        private readonly Random _random = new Random();

        public MissileInterceptor(Func<MissileDbContext> dbContextProvider, HashSet<string> policy)
        {
            _dbcontextprovider = dbContextProvider;
            _policy = policy;
        }

        public async Task AttemptInterceptAsync(MissileInfo missile)
        {
            if (_policy.Contains(missile.HitLocation))
            {
                Console.WriteLine($"[Intercept] Marking missile {missile.Id} for interception...");
                using (var context = _dbcontextprovider())
                {
                    var repository = new MissileRepository(context);
                    var dbMissile = repository.GetMissilesByHitLocation(missile.HitLocation)
                        .FirstOrDefault(m => m.Id == missile.Id);
                    if (dbMissile != null)
                    {
                        dbMissile.IsIntercepted = true;
                        await context.SaveChangesAsync();
                    }
                }

                await Task.Delay(3000);

                bool isSuccess = _random.Next(100) < 75; // 75% chance of success
                Console.WriteLine($"[Intercept] Success: {isSuccess}");

                using (var context = _dbcontextprovider())
                {
                    var repository = new MissileRepository(context);
                    var dbMissile = repository.GetMissilesByHitLocation(missile.HitLocation)
                        .FirstOrDefault(m => m.Id == missile.Id);
                    if (dbMissile != null)
                    {
                        dbMissile.InterceptSuccess = isSuccess;
                        await context.SaveChangesAsync();
                        Console.WriteLine(isSuccess 
                            ? $"[Intercept] Missile {missile.Id} intercepted successfully!" 
                            : $"[Intercept] Missile {missile.Id} interception failed!");
                    }
                }
            }
        }
    }
}
