using System.Text.Json;
using System.Text.Json.Serialization;
using Dapr;
using Dapr.Client;
using MassTransit;
using MassTransit.MongoDbIntegration;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using MongoDB.Entities;
using VietGeeks.TestPlatform.Integration.Contract;
using VietGeeks.TestPlatform.SharedKernel;
using VietGeeks.TestPlatform.SharedKernel.Events;

namespace VietGeeks.TestPlatform.AccountManager.Controllers;

[ApiController]
[Route("[controller]")]
public class WebhookController : ControllerBase
{
    private const string WebhookBus = "webhook-pubsub";

    private const string AccessCodeSendingStatusQueue = "access-code-email-notification";

    [Authorize( Policy = AuthPolicyNames.CreateUserPolicy )]
    [HttpPost("CreateUserRequest")]
    public async Task<IActionResult> CreateUserRequest( [FromBody] UserCreateRequest request,
        [FromServices] MongoDbContext dbContext,
        [FromServices] IPublishEndpoint publishEndpoint,
        [FromServices] ILogger<WebhookController> logger,
        CancellationToken cancellationToken)
    {
        logger.ReceivedCreatingUserAction(request.UserId, request.Email);

        // Could reuse object id from original source as id of entity User
        (string providerId, string userId) = request.ParseUserId();
        try
        {
            var userEntity = new Data.User
            {
                ID = userId, Email = request.Email, ProviderId = providerId,
            };

            await dbContext.BeginTransaction(cancellationToken);
            await DB.InsertAsync(userEntity, dbContext.Session, cancellationToken);
            await publishEndpoint.Publish(new UserCreatedEvent { UserId = userEntity.ID }, cancellationToken);
            await dbContext.CommitTransaction(cancellationToken);

            logger.CreatedUser(userEntity.ID, userEntity.Email);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            logger.DuplicatedUser(ex);

            return BadRequest("DuplicatedUserIdOrEmail");
        }

        return Accepted(request.UserId);
    }

    [HttpPost("Mailjet")]
    public async Task<IActionResult> Mailjet([FromServices] DaprClient client, [FromBody] MailjetEvent[] events)
    {
        foreach (var g in events.GroupBy(c => c.Payload).Where(c => !string.IsNullOrEmpty(c.Key)))
        {
            var state = await client.GetStateAsync<TestInvitationEventData>("general-notify-store", g.Key) ??
                        new TestInvitationEventData();

            state.Events.AddRange(g.Select(c => c.GetData()));
            await client.SaveStateAsync("general-notify-store", g.Key, state);
        }

        Console.WriteLine("processed event {0}", events.Count());

        return Ok();
    }

    [HttpGet("TestInvitationEvents")]
    public async Task<IActionResult> GetTestInvitationEvents([FromServices] DaprClient client,
        [FromQuery] string[] keys)
    {
        var result = new List<dynamic>();
        var states = await client.GetBulkStateAsync("general-notify-store", keys.ToList(), 0);
        foreach (var state in states)
        {
            var parsedEvents =
                JsonSerializer.Deserialize<TestInvitationEventData>(state.Value, client.JsonSerializerOptions);
            if (parsedEvents?.Events == null)
            {
                continue;
            }

            result.Add(new
            {
                UniqueId = state.Key, parsedEvents.Events
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

    public virtual IDictionary<string, string> GetData()
    {
        return new Dictionary<string, string>
        {
            { "email", email },
            { "time", time.ToString() }
        };
    }
}

public class MailjetSentEvent : MailjetEvent
{
    public string smtp_reply { get; set; } = default!;

    public override IDictionary<string, string> GetData()
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

    public override IDictionary<string, string> GetData()
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

    public override IDictionary<string, string> GetData()
    {
        var result = base.GetData();
        result["event"] = "spam";

        return result;
    }
}