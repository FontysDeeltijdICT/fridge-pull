using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
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
        public async Task GetLastestMeasurements_ExistentMacAddress_ShouldReturnCorrectData()
        {
            var firstMeasurement = _fixture.Measurements.First();
            var firstMeasurementMacAddress = firstMeasurement.Device;
            var firstMeasurementTimestamp = firstMeasurement.Time;
            var measurementsFromMacAddress = _fixture.Measurements.Where(m => m.Device == firstMeasurementMacAddress && m.Time == firstMeasurementTimestamp).ToArray();
            var client = _fixture.Factory.CreateClient();
            
            var response = await client.GetAsync($"/hardware/{firstMeasurementMacAddress}");
            var measurementsJson = await response.Content.ReadAsStringAsync();
            var measurements = JsonConvert.DeserializeObject<List<MeasurementJson>>(measurementsJson);

            Assert.Single(measurements);
            Assert.Equal(measurementsFromMacAddress.Length, measurements.Single().sensors.Count);

            foreach (var sensor in measurements.Single().sensors)
            {
                var expectedSensor = measurementsFromMacAddress.Single(m => m.Sensor == sensor.Id);
                
                Assert.Equal(expectedSensor.Temperature, sensor.Temperature);
                Assert.Equal(expectedSensor.Presence, sensor.Presence);
            }
        }
    }
}