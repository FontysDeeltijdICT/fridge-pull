using System;
using InfluxDB.Client.Core;

namespace FridgePull.InfluxDb.Models
{
    [InfluxDB.Client.Core.Measurement("beer")]
    public class Measurement
    {
        [Column("sensor", IsTag = true)]
        public int Id { get; set; }
        
        [Column("value")]
        public double Temperature { get; set; }
        
        [Column(IsTimestamp = true)]
        public DateTime Time { get; set; }
        
        [Column("presence")]
        public bool Presence { get; set; }
    }
}