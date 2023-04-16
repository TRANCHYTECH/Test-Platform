using System;
namespace VietGeeks.TestPlatform.ProctorManager.Api.Models;

public class AppConfigurationSettings
{
    public WebhookPubSubSettings WebhookPubSub { get; set; } = default!;
}

public class WebhookPubSubSettings
{
    public string WebhookBus { get; set; } = default!;
    public string AccessCodeSendingStatusQueue { get; set; } = default!;
    public string UserCreateRequestQueue { get; set; } = default!;
}