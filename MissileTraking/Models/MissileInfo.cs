using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MissileTracking.Models
{
    public class MissileInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Type { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public string HitLocation { get; set; }

        // Indicates if an interception was attempted
        public bool IsIntercepted { get; set; } = false;

        // Indicates if the interception was successful
        public bool InterceptSuccess { get; set; } = false;
    }
}