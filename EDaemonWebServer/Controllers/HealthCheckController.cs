using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using EDaemonWebServer.Services.Interfaces;
using System;

namespace EDaemonWebServer.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HealthCheckController : ControllerBase
    {
        private readonly IHealthCheckService _healthCheckService;

        public HealthCheckController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet("test-db-connection")]
        public async Task<IActionResult> TestDbConnection()
        {
            var (ok, message, scalar, error) = await _healthCheckService.TestDbConnectionAsync();

            if (ok)
                return Ok(new { ok = true, message, scalar });

            return StatusCode(500, new { ok = false, message, error });
        }
    }
}
