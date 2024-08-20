using System.Text.Json;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using VietGeeks.TestPlatform.Integration.Contract;

namespace VietGeeks.TestPlatform.TestManager.Functions.SendEmail;

public class SendTestAccessCode(ILoggerFactory loggerFactory)
{
    private readonly ILogger _logger = loggerFactory.CreateLogger<SendTestAccessCode>();

    [FunctionName("SendTestAccessCode")]
    public async Task Run([ServiceBusTrigger("send-test-access-code", Connection = "TestRunnerSB")] SendTestAccessCodeRequest invitation)
    {
        MailjetClient client = new(Environment.GetEnvironmentVariable("MJ_APIKEY_PUBLIC"), Environment.GetEnvironmentVariable("MJ_APIKEY_PRIVATE"));
        var mails = invitation.Receivers.Select(r => new TransactionalEmail
        {
            CustomID = r.AccessCode,
            EventPayload = invitation.GenerateReferenceId(r.AccessCode),
            From = new("notify@tranchy.tech", "Test Portal Notification"),
            To = new List<SendContact>
            {
                new(r.Email)
            },
            TemplateID = 4647722,
            TemplateLanguage = true,
            Subject = "Test Portal Notification",
            Variables = new Dictionary<string, object>
            {
                {"TestUrl",invitation.TestUrl },
                {"AccessCode", r.AccessCode }
            }
        });
        var sendResult = await client.SendTransactionalEmailsAsync(mails);
        foreach (var item in sendResult.Messages.Where(c => c.Status != "success"))
        {
            _logger.LogError("Failed to send test access code to email {0}", JsonSerializer.Serialize(item));
        }
    }
}

