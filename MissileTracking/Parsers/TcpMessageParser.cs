using MissileTracking.Models;

namespace MissileTracking.Parsers
{
    public class TcpMessageParser
    {
        /// <summary>
        /// Parses a missile information message.
        /// Expected format: "Type,X,Y,HitLocation"
        /// </summary>
        public static MissileInfo ParseMissileInfo(string data)
        {
            
            var parts = data.Split(',');
            if (parts.Length < 4)
            {
                throw new ArgumentException("Invalid missile data format.");
            }

            return new MissileInfo
            {
                Type = parts[0],
                X = int.Parse(parts[1]),
                Y = int.Parse(parts[2]),
                HitLocation = parts[3]
            };
        }
        

    }
}