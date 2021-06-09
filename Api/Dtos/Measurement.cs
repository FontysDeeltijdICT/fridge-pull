using System;
using System.Collections.Generic;

namespace FridgePull.Api.Dtos
{
    public class Measurement
    {
        public DateTime MeasuredAt { get; set; }
        public IEnumerable<Sensor> Sensors { get; set; }
    }
}