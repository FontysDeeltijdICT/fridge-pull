using System;
using System.Collections.Generic;
using FridgePull.Api;
using FridgePull.InfluxDb.Models;
using FridgePull.InfluxDb.Options;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace FridgePull.IntegrationTests.Fixture
{
    public class MeasurementFixture : IDisposable
    {
        public readonly WebApplicationFactory<Startup> Factory;
        public List<Measurement> Measurements;
        
        private readonly InfluxDBClient _influxDbClient;
        private readonly InfluxDbOptions _influxDbOptions;
        private readonly BierCoolOptions _bierCoolOptions;

        public MeasurementFixture()
        {
            Factory = new WebApplicationFactory<Startup>();

            _influxDbClient = Factory.Services.GetService<InfluxDBClient>();
            _influxDbOptions = Factory.Services.GetService<IOptions<InfluxDbOptions>>().Value;
            _bierCoolOptions = Factory.Services.GetService<IOptions<BierCoolOptions>>().Value;
            
            SeedInfluxDb();
        }

        private async void SeedInfluxDb()
        {
            CreateMeasurementsToSeed();
            
            var writeApi = _influxDbClient.GetWriteApiAsync();

            await writeApi.WriteMeasurementsAsync(_influxDbOptions.Bucket, _influxDbOptions.Organization, WritePrecision.S, Measurements);
        }

        private void CreateMeasurementsToSeed()
        {
            const string device1MacAddress = "ab:cd:ef:12:34";
            const string device2MacAddress = "ac:bd:fe:32:14";
            
            var dateTimeNow = DateTime.UtcNow;
            var dateTimeEarlier = dateTimeNow.AddMinutes(-5);

            Measurements = new List<Measurement>
            {
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 1,
                    Presence = true,
                    Temperature = 7.2,
                    Time = dateTimeNow,
                    Version = _bierCoolOptions.Version
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 2,
                    Presence = false,
                    Temperature = 5,
                    Time = dateTimeNow,
                    Version = _bierCoolOptions.Version
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 3,
                    Presence = true,
                    Temperature = 11.2,
                    Time = dateTimeNow,
                    Version = _bierCoolOptions.Version
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 1,
                    Presence = false,
                    Temperature = 7.4,
                    Time = dateTimeEarlier,
                    Version = _bierCoolOptions.Version
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 2,
                    Presence = false,
                    Temperature = 5.2,
                    Time = dateTimeEarlier,
                    Version = _bierCoolOptions.Version
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 3,
                    Presence = true,
                    Temperature = 11.6,
                    Time = dateTimeEarlier,
                    Version = _bierCoolOptions.Version
                },
                new()
                {
                    Device = device2MacAddress,
                    Sensor = 1,
                    Presence = false,
                    Temperature = 5.3,
                    Time = dateTimeNow,
                    Version = _bierCoolOptions.Version
                },
                new()
                {
                    Device = device2MacAddress,
                    Sensor = 2,
                    Presence = false,
                    Temperature = 5.0,
                    Time = dateTimeNow,
                    Version = _bierCoolOptions.Version
                },
                new()
                {
                    Device = device2MacAddress,
                    Sensor = 3,
                    Presence = false,
                    Temperature = 5.1,
                    Time = dateTimeNow,
                    Version = _bierCoolOptions.Version
                }
            };
        }

        public void Dispose()
        {
            _influxDbClient.GetDeleteApi()
                .Delete(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow, "", _influxDbOptions.Bucket, _influxDbOptions.Organization);
            _influxDbClient?.Dispose();
        }
    }
}