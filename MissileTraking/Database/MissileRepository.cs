using Microsoft.EntityFrameworkCore;
using MissileTracking.Models;

namespace MissileTracking.Database
{
    /// <summary>
    /// Repository for accessing missile data using a database.
    /// Implements an event-based mechanism to notify subscribers when new data is added.
    /// </summary>
    public class MissileRepository
    {
        private readonly MissileDbContext _context;

        /// <summary>
        /// Event raised when a new missile record is added.
        /// </summary>
        public event EventHandler<MissileEventArgs>? MissileAdded;

        public MissileRepository(MissileDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds missile info to the database and raises the MissileAdded event.
        /// </summary>
        public async Task<bool> AddMissileAsync(MissileInfo missile)
        {
            if (missile == null)
            {
                throw new ArgumentNullException(nameof(missile), "Missile cannot be null.");
            }

            try
            {
                await _context.Missiles.AddAsync(missile);
                await _context.SaveChangesAsync();
                
                // Safely raise the event
                OnMissileAdded(missile);

                return true; // Indicate success
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to add missile: {ex.Message}");
                return false; // Indicate failure
            }
        }

        /// <summary>
        /// Returns all missile info records that have a specific hit location.
        /// Uses LINQ to query the database asynchronously.
        /// </summary>
        public async Task<List<MissileInfo>> GetMissilesByHitLocationAsync(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                throw new ArgumentException("Location cannot be null or empty.", nameof(location));
            }

            try
            {
                return await _context.Missiles
                    .Where(m => m.HitLocation == location)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to retrieve missiles for location {location}: {ex.Message}");
                return new List<MissileInfo>(); // Return an empty list to avoid null reference issues
            }
        }
        
        public async Task<MissileInfo?> GetMissileByIdAsync(int missileId)
        {
            try
            {
                return await _context.Missiles.FirstOrDefaultAsync(m => m.Id == missileId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[ERROR] Failed to retrieve missile with ID {missileId}: {ex.Message}");
                return null;
            }
        }
        
        protected virtual void OnMissileAdded(MissileInfo missile)
        {
            var handler = MissileAdded; 
            handler?.Invoke(this, new MissileEventArgs { Missile = missile });
        }
    }
}


