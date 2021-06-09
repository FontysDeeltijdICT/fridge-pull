using System.Collections.Generic;
using System.Linq;
using FridgePull.InfluxDb;
using FridgePull.InfluxDb.Models;
using Microsoft.AspNetCore.Mvc;

namespace FridgePull.Api.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class HardwareController : ControllerBase
    {
        private readonly MeasurementService _measurementService;

        public HardwareController(MeasurementService measurementService)
        {
            _measurementService = measurementService;
        }

        [HttpGet("{macAddress}")]
        public ActionResult<IEnumerable<Measurement>> GetLatestMeasurement(string macAddress)
        {
            var measurements = _measurementService.GetLastMeasurement(macAddress);

            if (measurements == null || !measurements.Any())
            {
                return NotFound();
            }

            return Ok(measurements);
        }
    }
}