using FridgePull.Api.Dtos;
using Measurement = FridgePull.InfluxDb.Models.Measurement;

namespace FridgePull.Api.Utilities.Mappers
{
    public static class SensorMapper
    {
        public static Sensor ToDto(Measurement measurement) => new()
        {
            Id = measurement.Sensor,
            Temperature = measurement.Temperature,
            Presence = measurement.Presence
        };
    }
}