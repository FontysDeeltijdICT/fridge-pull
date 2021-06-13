using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FridgePull.InfluxDb.Models;
using FridgePull.IntegrationTests.Fixture;
using Newtonsoft.Json;
using Xunit;

namespace FridgePull.IntegrationTests
{
    public class MeasurementTests : IClassFixture<MeasurementFixture>
    {
        private readonly MeasurementFixture _fixture;

        public MeasurementTests(MeasurementFixture fixture)
        {
            _fixture = fixture;
        }

        [Fact]
        public async Task GetLatestMeasurements_NonExistentMacAddress_ShouldReturnNotFound()
        {
            var client = _fixture.Factory.CreateClient();

            var response = await client.GetAsync("/hardware/doesnotexist");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetLatestMeasurements_ExistentMacAddress_ShouldReturnOK()
        {
            var existentMacAddress = _fixture.Measurements.First().Device;
            var client = _fixture.Factory.CreateClient();

            var response = await client.GetAsync($"/hardware/{existentMacAddress}");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetLastestMeasurements_ExistentMacAddress_ShouldReturnOneMeasurement()
        {
            var existentMacAddress = _fixture.Measurements.First().Device;
            var client = _fixture.Factory.CreateClient();

            var response = await client.GetAsync($"/hardware/{existentMacAddress}");
            var measurementsJson = await response.Content.ReadAsStringAsync();
            var measurements = JsonConvert.DeserializeObject<List<MeasurementJson>>(measurementsJson);

            Assert.Single(measurements);
        }

        [Fact]
        public async Task GetLastestMeasurements_ExistentMacAddress_ShouldReturnCorrectAmountOfSensors()
        {
            var firstMeasurement = _fixture.Measurements.First();
            var firstMeasurementMacAddress = firstMeasurement.Device;
            var firstMeasurementTimestamp = firstMeasurement.Time;
            var measurementsFromMacAddressAtTimestamp = _fixture
                .Measurements
                .Where(m => m.Device == firstMeasurementMacAddress && m.Time == firstMeasurementTimestamp)
                .ToArray();
            var client = _fixture.Factory.CreateClient();

            var response = await client.GetAsync($"/hardware/{firstMeasurementMacAddress}");
            var measurementsJson = await response.Content.ReadAsStringAsync();
            var measurements = JsonConvert.DeserializeObject<List<MeasurementJson>>(measurementsJson);

            Assert.Equal(measurementsFromMacAddressAtTimestamp.Length, measurements.Single().sensors.Count);
        }

        [Fact]
        public async Task GetLastestMeasurements_ExistentMacAddress_ShouldContainCorrectSensorIds()
        {
            var firstMeasurement = _fixture.Measurements.First();
            var firstMeasurementMacAddress = firstMeasurement.Device;
            var firstMeasurementTimestamp = firstMeasurement.Time;
            var expectedMeasurementSensors = _fixture
                .Measurements
                .Where(m => m.Device == firstMeasurementMacAddress && m.Time == firstMeasurementTimestamp)
                .Select(m => m.Sensor)
                .OrderBy(s => s)
                .ToArray();
            var client = _fixture.Factory.CreateClient();

            var response = await client.GetAsync($"/hardware/{firstMeasurementMacAddress}");
            var measurementsJson = await response.Content.ReadAsStringAsync();
            var measurements = JsonConvert.DeserializeObject<List<MeasurementJson>>(measurementsJson);
            var actualMeasurementSensors = measurements.Single()
                .sensors
                .Select(s => s.Id)
                .OrderBy(s => s)
                .ToArray();

            Assert.Equal(expectedMeasurementSensors, actualMeasurementSensors);
        }

        [Fact]
        public async Task GetLastestMeasurements_ExistentMacAddress_ShouldContainCorrectTemperatures()
        {
            var firstMeasurement = _fixture.Measurements.First();
            var firstMeasurementMacAddress = firstMeasurement.Device;
            var firstMeasurementTimestamp = firstMeasurement.Time;
            var measurementsFromMacAddress = _fixture.Measurements
                .Where(m => m.Device == firstMeasurementMacAddress && m.Time == firstMeasurementTimestamp).ToArray();
            var client = _fixture.Factory.CreateClient();

            var response = await client.GetAsync($"/hardware/{firstMeasurementMacAddress}");
            var measurementsJson = await response.Content.ReadAsStringAsync();
            var measurements = JsonConvert.DeserializeObject<List<MeasurementJson>>(measurementsJson);

            foreach (var expectedMeasurement in measurementsFromMacAddress)
            {
                var actualMeasurement = measurements.Single().sensors.Single(m => m.Id == expectedMeasurement.Sensor);

                Assert.Equal(expectedMeasurement.Temperature, actualMeasurement.Temperature);
            }
        }

        [Fact]
        public async Task GetLastestMeasurements_ExistentMacAddress_ShouldContainCorrectPresences()
        {
            var firstMeasurement = _fixture.Measurements.First();
            var firstMeasurementMacAddress = firstMeasurement.Device;
            var firstMeasurementTimestamp = firstMeasurement.Time;
            var measurementsFromMacAddress = _fixture.Measurements
                .Where(m => m.Device == firstMeasurementMacAddress && m.Time == firstMeasurementTimestamp).ToArray();
            var client = _fixture.Factory.CreateClient();

            var response = await client.GetAsync($"/hardware/{firstMeasurementMacAddress}");
            var measurementsJson = await response.Content.ReadAsStringAsync();
            var measurements = JsonConvert.DeserializeObject<List<MeasurementJson>>(measurementsJson);

            foreach (var expectedMeasurement in measurementsFromMacAddress)
            {
                var actualMeasurement = measurements.Single().sensors.Single(m => m.Id == expectedMeasurement.Sensor);

                Assert.Equal(expectedMeasurement.Presence, actualMeasurement.Presence);
            }
        }
    }
}