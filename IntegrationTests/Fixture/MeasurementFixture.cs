using System;
using System.Collections.Generic;
using FridgePull.Api;
using FridgePull.InfluxDb.Models;
using InfluxDB.Client;
using InfluxDB.Client.Api.Domain;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace FridgePull.IntegrationTests.Fixture
{
    public class MeasurementFixture : IDisposable
    {
        public readonly WebApplicationFactory<Startup> Factory;
        public List<Measurement> Measurements;
        
        private readonly InfluxDBClient _influxDbClient;

        public MeasurementFixture()
        {
            Factory = new WebApplicationFactory<Startup>();

            _influxDbClient = Factory.Services.GetService<InfluxDBClient>();
            
            SeedInfluxDb();
        }

        private async void SeedInfluxDb()
        {
            CreateMeasurementsToSeed();
            
            var writeApi = _influxDbClient.GetWriteApiAsync();

            // TODO: Extract bucket/org
            await writeApi.WriteMeasurementsAsync("biercool", "biercool", WritePrecision.S, Measurements);
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
                    Version = "1"
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 2,
                    Presence = false,
                    Temperature = 5,
                    Time = dateTimeNow,
                    Version = "1"
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 3,
                    Presence = true,
                    Temperature = 11.2,
                    Time = dateTimeNow,
                    Version = "1"
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 1,
                    Presence = false,
                    Temperature = 7.4,
                    Time = dateTimeEarlier,
                    Version = "1"
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 2,
                    Presence = false,
                    Temperature = 5.2,
                    Time = dateTimeEarlier,
                    Version = "1"
                },
                new()
                {
                    Device = device1MacAddress,
                    Sensor = 3,
                    Presence = true,
                    Temperature = 11.6,
                    Time = dateTimeEarlier,
                    Version = "1"
                },
                new()
                {
                    Device = device2MacAddress,
                    Sensor = 1,
                    Presence = false,
                    Temperature = 5.3,
                    Time = dateTimeNow,
                    Version = "1"
                },
                new()
                {
                    Device = device2MacAddress,
                    Sensor = 2,
                    Presence = false,
                    Temperature = 5.0,
                    Time = dateTimeNow,
                    Version = "1"
                },
                new()
                {
                    Device = device2MacAddress,
                    Sensor = 3,
                    Presence = false,
                    Temperature = 5.1,
                    Time = dateTimeNow,
                    Version = "1"
                }
            };
        }

        public void Dispose()
        {
            // TODO: Extract bucket/org
            _influxDbClient.GetDeleteApi()
                .Delete(DateTime.UtcNow.AddYears(-1), DateTime.UtcNow, "", "biercool", "biercool");
            _influxDbClient?.Dispose();
        }
    }
}