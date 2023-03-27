using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;


namespace VietGeeks.TestPlatform.ProctorManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WebhookController : ControllerBase
    {
        private const string MailNotifyBus = "access-code-email-notify-pubsub";
        private const string NotifyQueue = "access-code-email-notification";

        private readonly ILogger<WebhookController> _logger;

        public WebhookController(ILogger<WebhookController> logger)
        {
            _logger = logger;
        }

        [HttpPost("Mailjet")]
        public async Task<IActionResult> Mailjet([FromServices] DaprClient client, [FromBody] MailjetEvent[] events)
        {
            await client.PublishEventAsync(MailNotifyBus, NotifyQueue, System.Text.Json.JsonSerializer.Serialize(events));

            return Ok();
        }

        [Topic(MailNotifyBus, NotifyQueue)]
        [HttpPost("MailjetEventReceived")]
        public IActionResult ProcessMailjetEvents([FromBody]string @event)
        {
            var parsedEvents = System.Text.Json.JsonSerializer.Deserialize<MailjetEvent[]>(@event);
            Console.WriteLine("processed event {0}", parsedEvents?.Count());

            return Ok();
        }
    }

    [JsonPolymorphic(TypeDiscriminatorPropertyName = "event")]
    [JsonDerivedType(typeof(MailjetSentEvent), "sent")]
    [JsonDerivedType(typeof(MailjetBlockedEvent), "blocked")]
    [JsonDerivedType(typeof(MailjetSpamEvent), "spam")]
    public abstract class MailjetEvent
    {
        public int time { get; set; } = default!;
        public long MessageID { get; set; } = default!;
        public string Message_GUID { get; set; } = default!;
        public string email { get; set; } = default!;
        public string CustomID { get; set; } = default!;
        public int mj_campaign_id { get; set; } = default!;
        public int mj_contact_id { get; set; } = default!;
        public string customcampaign { get; set; } = default!;
    }

    public class MailjetSentEvent : MailjetEvent
    {
        public string smtp_reply { get; set; } = default!;
    }

    public class MailjetBlockedEvent : MailjetEvent
    {
        public string Payload { get; set; } = default!;
        public string error_related_to { get; set; } = default!;
        public string error { get; set; } = default!;
    }

    public class MailjetSpamEvent : MailjetEvent
    {
        public string Payload { get; set; } = default!;
        public string source { get; set; } = default!;
    }
}

