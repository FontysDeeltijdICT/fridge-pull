using System;
using System.Collections.Generic;

namespace FridgePull.IntegrationTests
{
    public class MeasurementJson
    {
        public DateTime MeasuredAt { get; set; }
        public IList<SensorsJson> sensors { get; set; }
    }

    public class SensorsJson
    {
        public int Id { get; set; }
        public double Temperature { get; set; }
        public bool Presence { get; set; }
    }
}