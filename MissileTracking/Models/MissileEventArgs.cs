namespace MissileTracking.Models
{
    /// <summary>
    /// EventArgs class for missile events.
    /// </summary>
    public class MissileEventArgs : EventArgs
    {
        public MissileInfo Missile { get; set; }
    }
}