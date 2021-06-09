using System;
using InfluxDB.Client.Core;

namespace FridgePull.InfluxDb.Models
{
    [InfluxDB.Client.Core.Measurement("beer")]
    public class Measurement
    {
        [Column("device", IsTag = true)]
        public string Device { get; set; }
        
        [Column("sensor", IsTag = true)]
        public int Sensor { get; set; }
        
        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
        
        [Column("value")]
        public double Temperature { get; set; }
        
        [Column("presence", IsTag = true)]
        public bool Presence { get; set; }
        
        [Column("version", IsTag = true)]
        public string Version { get; set; }
    }
}