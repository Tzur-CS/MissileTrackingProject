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
        public event EventHandler<MissileEventArgs> MissileAdded;

        public MissileRepository(MissileDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Adds missile info to the real database and raises the MissileAdded event.
        /// </summary>
        public async Task AddMissileAsync(MissileInfo missile)
        {
            await _context.Missiles.AddAsync(missile);
            await _context.SaveChangesAsync();
            OnMissileAdded(missile);
        }

        /// <summary>
        /// Raises the MissileAdded event.
        /// </summary>
        protected virtual void OnMissileAdded(MissileInfo missile)
        {
            MissileAdded?.Invoke(this, new MissileEventArgs { Missile = missile });
        }

        /// <summary>
        /// Returns all missile info records that have a specific hit location.
        /// Uses LINQ to query the database.
        /// </summary>
        public IEnumerable<MissileInfo> GetMissilesByHitLocation(string location)
        {
            return _context.Missiles.Where(m => m.HitLocation == location).ToList();
        }
    }
}