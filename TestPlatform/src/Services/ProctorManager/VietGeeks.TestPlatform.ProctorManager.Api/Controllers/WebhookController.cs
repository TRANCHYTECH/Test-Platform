using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.Integration.Contract;
using VietGeeks.TestPlatform.Integration.Contracts;
namespace VietGeeks.TestPlatform.ProctorManager.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class WebhookController : ControllerBase
{
    private const string WebhookBus = "webhook-pubsub";

    private const string AccessCodeSendingStatusQueue = "access-code-email-notification";

    private const string UserCreateRequestQueue = "user-create-request";

    private readonly ILogger<WebhookController> _logger;

    public WebhookController(ILogger<WebhookController> logger)
    {
        _logger = logger;
    }

    [HttpPost("CreateUserRequest")]
    public async Task<IActionResult> CreateUserRequest([FromServices] DaprClient client, [FromBody] UserCreateRequest request)
    {
        await client.PublishEventAsync(WebhookBus, UserCreateRequestQueue, JsonSerializer.Serialize(request));

        return Ok();
    }

    [HttpPost("Mailjet")]
    public async Task<IActionResult> Mailjet([FromServices] DaprClient client, [FromBody] MailjetEvent[] events)
    {
        await client.PublishEventAsync(WebhookBus, AccessCodeSendingStatusQueue, JsonSerializer.Serialize(events));

        return Ok();
    }

    // [Topic(WebhookBus, UserCreateRequestQueue)]
    // [HttpPost("ProcessUserCreateRequest")]
    // public IActionResult ProcessUserCreateRequest([FromBody] string @event)
    // {
    //     var parsedEvent = JsonSerializer.Deserialize<UserCreateRequest>(@event) ?? throw new Exception("Wrong events");

       
    //     Console.WriteLine("processed event {0}", parsedEvent.UserId);

    //     return Ok();
    // }

    [Topic(WebhookBus, AccessCodeSendingStatusQueue)]
    [HttpPost("ProcessMailjetEvents")]
    public async Task<IActionResult> ProcessMailjetEvents([FromServices] DaprClient client, [FromBody] string @event)
    {
        var parsedEvents = JsonSerializer.Deserialize<MailjetEvent[]>(@event) ?? throw new Exception("Wrong events");
        foreach (var g in parsedEvents.GroupBy(c => c.Payload).Where(c => !string.IsNullOrEmpty(c.Key)))
        {
            var state = await client.GetStateAsync<TestInvitiationEventData>("general-notify-store", g.Key) ?? new TestInvitiationEventData();

            state.Events.AddRange(g.Select(c => c.GetData()));
            await client.SaveStateAsync("general-notify-store", g.Key, state);
        };
        Console.WriteLine("processed event {0}", parsedEvents.Count());

        return Ok();
    }

    [HttpGet("TestInvitationEvents")]
    public async Task<IActionResult> GetTestInvitationEvents([FromServices] DaprClient client, [FromQuery] string[] keys)
    {
        var result = new List<dynamic>();
        IReadOnlyList<BulkStateItem> states = await client.GetBulkStateAsync("general-notify-store", keys.ToList(), 0);
        foreach (var state in states)
        {
            var parsedEvents = JsonSerializer.Deserialize<TestInvitiationEventData>(state.Value, client.JsonSerializerOptions);
            if (parsedEvents == null)
            {
                continue;
            }
            if (parsedEvents.Events == null)
            {
                continue;
            }

            result.Add(new
            {
                UniqueId = state.Key,
                Events = parsedEvents.Events
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
    public long time { get; set; } = default!;
    public long MessageID { get; set; } = default!;
    public string Message_GUID { get; set; } = default!;
    public string email { get; set; } = default!;
    public string CustomID { get; set; } = default!;
    public int mj_campaign_id { get; set; } = default!;
    public int mj_contact_id { get; set; } = default!;
    public string customcampaign { get; set; } = default!;
    public string Payload { get; set; } = default!;

    public virtual Dictionary<string, string> GetData() => new Dictionary<string, string>{
                        {"email", email},
                        {"time", time.ToString()}
                    };
}

public class MailjetSentEvent : MailjetEvent
{
    public string smtp_reply { get; set; } = default!;

    public override Dictionary<string, string> GetData()
    {
        var result = base.GetData();
        result["event"] = "sent";
        return result;
    }
}

public class MailjetBlockedEvent : MailjetEvent
{
    public string error_related_to { get; set; } = default!;
    public string error { get; set; } = default!;

    public override Dictionary<string, string> GetData()
    {
        var result = base.GetData();
        result["event"] = "blocked";
        result["error"] = error;

        return result;
    }
}

public class MailjetSpamEvent : MailjetEvent
{
    public string source { get; set; } = default!;

    public override Dictionary<string, string> GetData()
    {
        var result = base.GetData();
        result["event"] = "spam";

        return result;
    }
}

