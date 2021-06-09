using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FridgePull.Api;
using FridgePull.IntegrationTests.Fixture;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using Xunit;

namespace FridgePull.IntegrationTests
{
    public class MeasurementTests : MeasurementFixture
    {
        public MeasurementTests(WebApplicationFactory<Startup> factory) : base(factory) {}
        
        [Fact]
        public async Task GetLatestMeasurements_NonExistentMacAddress_ShouldReturnNotFound()
        {
            var client = Factory.CreateClient();

            var response = await client.GetAsync("/hardware/doesnotexist");
            
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetLatestMeasurements_ExistentMacAddress_ShouldReturnOK()
        {
            var existentMacAddress = Measurements.First().Device;
            var client = Factory.CreateClient();

            var response = await client.GetAsync($"/hardware/{existentMacAddress}");

            response.EnsureSuccessStatusCode();
        }

        [Fact]
        public async Task GetLastestMeasurements_ExistentMacAddress_ShouldReturnCorrectData()
        {
            var firstMeasurement = Measurements.First();
            var firstMeasurementMacAddress = firstMeasurement.Device;
            var firstMeasurementTimestamp = firstMeasurement.Time;
            var measurementsFromMacAddress = Measurements.Where(m => m.Device == firstMeasurementMacAddress && m.Time == firstMeasurementTimestamp).ToArray();
            var client = Factory.CreateClient();
            
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