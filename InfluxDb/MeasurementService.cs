using System;
using System.Collections.Generic;
using FridgePull.InfluxDb.Models;

namespace FridgePull.InfluxDb
{
    public class MeasurementService
    {
        private readonly MeasurementRepository _measurementRepository;

        public MeasurementService(MeasurementRepository measurementRepository)
        {
            _measurementRepository = measurementRepository;
        }

        public IEnumerable<Measurement> GetLastMeasurement(string macAddress)
        {
            var fromDateTime = DateTime.Now.AddMonths(-1);

            return GetLastMeasurement(macAddress, fromDateTime);
        }

        public IEnumerable<Measurement> GetLastMeasurement(string macAddress, DateTime fromDateTime)
        {
            return _measurementRepository.GetLastMeasurement(macAddress, fromDateTime);
        }
    }
}