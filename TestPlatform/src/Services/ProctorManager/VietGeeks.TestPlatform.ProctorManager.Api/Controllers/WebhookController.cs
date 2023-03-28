using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
        //private const string NotifyQueue = "access-code-email-notification_local";

#if DEBUG
        private const string NotifyQueue = "access-code-email-notification_local";
#else
        private const string NotifyQueue = "access-code-email-notification";
#endif

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
        [HttpPost("ProcessMailjetEvents")]
        public async Task<IActionResult> ProcessMailjetEvents([FromServices] DaprClient client, [FromBody] string @event)
        {
            var parsedEvents = JsonSerializer.Deserialize<MailjetEvent[]>(@event) ?? throw new Exception("Wrong events");
            foreach (var g in parsedEvents.GroupBy(c => c.CustomID))
            {
                var state = await client.GetStateAsync<TestInvitiationEvent>("GeneralNotifyStore", g.Key) ?? new TestInvitiationEvent();
                state.Events.AddRange(g.ToList());
                await client.SaveStateAsync("GeneralNotifyStore", g.Key, state);
            };
            Console.WriteLine("processed event {0}", parsedEvents.Count());
            return Ok();
        }

        [HttpGet("TestInvitationEvents")]
        public async Task<IActionResult> GetTestInvitationEvents([FromServices] DaprClient client, [FromQuery]string[] keys)
        {
            var result = new List<AggregatedTestInvitationEvent>();
            IReadOnlyList<BulkStateItem> states = await client.GetBulkStateAsync("GeneralNotifyStore", keys.ToList(), 0);
            foreach (var state in states)
            {
                var parsedEvents = JsonSerializer.Deserialize<TestInvitiationEvent>(state.Value);
                if (parsedEvents == null)
                {
                    continue;
                }
                if (parsedEvents.Events == null)
                {
                    continue;
                }

                result.Add(new AggregatedTestInvitationEvent
                {
                    UniqueId = state.Key,
                    Statuses =parsedEvents.Events.Select(c => new { Time = c.time, Type = c.GetType().Name }).ToArray()
                });
            }

            return Ok(result);
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

    public class TestInvitiationEvent
    {
        public List<MailjetEvent> Events { get; set; } = new List<MailjetEvent>();
    }

    public class AggregatedTestInvitationEvent
    {
        public string UniqueId { get; set; } = default!;

        public dynamic[] Statuses { get; set; } = default!;
    }
}

