namespace MissileTracking.Models
{

    public class MissileEventArgs : EventArgs
    {
        public MissileInfo Missile { get; set; }
    }
}