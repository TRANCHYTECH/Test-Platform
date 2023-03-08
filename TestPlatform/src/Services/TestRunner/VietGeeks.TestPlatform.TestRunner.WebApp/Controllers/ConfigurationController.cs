using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VietGeeks.TestPlatform.TestRunner.WebApp.Models;

namespace VietGeeks.TestPlatform.TestRunner.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly AppConfigurationSettings _options;

        public ConfigurationController(ILogger<ConfigurationController> logger, IOptions<AppConfigurationSettings> options)
        {
            _logger = logger;
            _options = options.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_options);
        }
    }
}