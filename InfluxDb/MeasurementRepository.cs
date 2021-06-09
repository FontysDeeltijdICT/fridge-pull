using System;
using System.Collections.Generic;
using System.Xml;
using FridgePull.InfluxDb.Models;
using InfluxDB.Client;
using Microsoft.Extensions.Options;

namespace FridgePull.InfluxDb
{
    public class MeasurementRepository
    {
        private readonly BierCoolOptions _bierCoolOptions;
        private readonly InfluxDBClient _influxDbClient;
        private readonly InfluxDbOptions _influxDbOptions;

        public MeasurementRepository(
            InfluxDBClient influxDbClient, 
            IOptions<InfluxDbOptions> influxDbOptions, 
            IOptions<BierCoolOptions> bierCoolOptions
        )
        {
            _influxDbClient = influxDbClient;
            _influxDbOptions = influxDbOptions.Value;
            _bierCoolOptions = bierCoolOptions.Value;
        }

        public IEnumerable<Measurement> GetLastMeasurement(string macAddress, DateTime from)
        {
            var fromDateTimeString = XmlConvert.ToString(from, XmlDateTimeSerializationMode.Utc);
            
            var queryBuilder = new FluxQueryBuilder(_influxDbOptions.Bucket);
            var fluxQuery = queryBuilder
                .AddRange(fromDateTimeString)
                .AddFilter("_measurement", _bierCoolOptions.Measurement)
                .AddFilter("device", macAddress)
                .AddFilter("version", _bierCoolOptions.Version)
                .Last()
                .GetResult();

            // TODO: Async
            var queryApi = _influxDbClient.GetQueryApiSync();

            return queryApi.QuerySync<Measurement>(fluxQuery, _influxDbOptions.Organization);
        }
    }
}