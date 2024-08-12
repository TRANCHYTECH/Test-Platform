using System.Text.Json;
using Dapr;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.AccountManager.Services;
using VietGeeks.TestPlatform.Integration.Contracts;

namespace VietGeeks.TestPlatform.AccountManager.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountProcessor : ControllerBase
{
    private const string WebhookBus = "webhook-pubsub";

    private const string UserCreateRequestQueue = "user-create-request";

    [Topic(WebhookBus, UserCreateRequestQueue)]
    [HttpPost("ProcessUserCreateRequest")]
    public async Task<IActionResult> ProcessUserCreateRequest([FromServices] IAccountSettingsService accountSettingsService, [FromBody] string request)
    {
        var parsedRequest = JsonSerializer.Deserialize<UserCreateRequest>(request) ?? throw new Exception("Wrong events");
        await accountSettingsService.CreateUserProfile(new() { UserId = parsedRequest.UserId, Email = parsedRequest.Email });

        Console.WriteLine("processed event {0}", parsedRequest.UserId);

        return Ok();
    }
}
