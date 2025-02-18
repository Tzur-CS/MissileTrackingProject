using MissileTracking.Database;
using MissileTracking.Models;

namespace MissileTracking.Interception
{
    public class MissileInterceptorLogic
    {
        private readonly Func<MissileDbContext> _dbcontextprovider;
        private readonly HashSet<string> _policy;
        private readonly Random _random = new Random();

        public MissileInterceptorLogic(Func<MissileDbContext> dbContextProvider, HashSet<string> policy)
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
                    var dbMissile = await repository.GetMissileByIdAsync(missile.Id);

                    if (dbMissile == null)
                    {
                        Console.WriteLine($"[Intercept] Missile {missile.Id} not found in database.");
                        return; // Exit early if the missile doesn't exist
                    }

                    // Step 1: Mark as intercepted
                    dbMissile.IsIntercepted = true;
                    await context.SaveChangesAsync(); // Save the first change

                    // Step 2: Simulate interception process
                    await Task.Delay(3000);

                    // Step 3: Determine interception success
                    bool isSuccess = _random.Next(100) < 75; // 75% chance of success
                    dbMissile.InterceptSuccess = isSuccess;
                    await context.SaveChangesAsync(); // Save the second change

                    Console.WriteLine(isSuccess 
                        ? $"[Intercept] Missile {missile.Id} intercepted successfully! \u2705 " 
                        : $"[Intercept] Missile {missile.Id} interception failed! \u274c  ");
                }
            }
        }
    }
}
