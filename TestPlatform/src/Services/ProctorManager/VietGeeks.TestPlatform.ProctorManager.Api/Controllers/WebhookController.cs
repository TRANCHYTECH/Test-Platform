using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;


namespace VietGeeks.TestPlatform.ProctorManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private readonly ILogger<WebhookController> _logger;

        public WebhookController(ILogger<WebhookController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Mailjet")]
        public async Task<IActionResult> MailJet()
        {
            var request = await GetRawBodyAsync(Request);
            _logger.LogInformation("Message read {reques}", request);

            return Ok();
        }

        public static async Task<string> GetRawBodyAsync(HttpRequest request, Encoding? encoding = null)
        {
            if (!request.Body.CanSeek)
            {
                request.EnableBuffering();
            }

            request.Body.Position = 0;

            var reader = new StreamReader(request.Body, encoding ?? Encoding.UTF8);

            var body = await reader.ReadToEndAsync().ConfigureAwait(false);

            request.Body.Position = 0;

            return body;
        }
    }
}

