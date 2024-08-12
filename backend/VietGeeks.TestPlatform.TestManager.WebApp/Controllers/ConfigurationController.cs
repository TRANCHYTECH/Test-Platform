using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using VietGeeks.TestPlatform.TestManager.WebApp.Models;

namespace VietGeeks.TestPlatform.TestManager.WebApp.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ConfigurationController : ControllerBase
    {
        private readonly ILogger<ConfigurationController> _logger;
        private readonly AppConfigurationSettings _option;

        public ConfigurationController(ILogger<ConfigurationController> logger, IOptions<AppConfigurationSettings> option)
        {
            _logger = logger;
            _option = option.Value;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_option);
        }
    }
}