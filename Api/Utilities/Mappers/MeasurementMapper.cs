using System.Collections.Generic;
using System.Linq;
using FridgePull.Api.Dtos;

namespace FridgePull.Api.Utilities.Mappers
{
    public static class MeasurementMapper
    {
        public static IEnumerable<Measurement> ToDto(IEnumerable<InfluxDb.Models.Measurement> measurements)
        {
            var measurementsByTime = measurements.GroupBy(m => m.Time);

            foreach (var measurementsGroup in measurementsByTime)
            {
                var sensors = measurementsGroup.Select(SensorMapper.ToDto);

                yield return new Measurement
                {
                    MeasuredAt = measurementsGroup.First().Time,
                    Sensors = sensors
                };
            }
        }
    }
}