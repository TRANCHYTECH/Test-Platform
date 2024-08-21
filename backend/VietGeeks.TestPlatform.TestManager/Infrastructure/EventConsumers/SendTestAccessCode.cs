using System.Text.Json;
using Mailjet.Client;
using Mailjet.Client.TransactionalEmails;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using VietGeeks.TestPlatform.Integration.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.EventConsumers
{
    public class SendTestAccessCode(ILogger<SendTestAccessCode> logger, IConfiguration configuration) : IConsumer<SendTestAccessCodeRequest>
    {
        public async Task Consume(ConsumeContext<SendTestAccessCodeRequest> context)
        {
            logger.ReceivedEventSendingAccessCode();
            MailjetClient client = new(configuration.GetValue<string>("MailJet:PublicKey"), configuration.GetValue<string>("MailJet:PrivateKey"));
            var mails = context.Message.Receivers.Select(r => new TransactionalEmail
            {
                CustomID = r.AccessCode,
                EventPayload = context.Message.GenerateReferenceId(r.AccessCode),
                From = new SendContact("notify@tranchy.tech", "Test Portal Notification"),
                To = [new SendContact(r.Email)],
                TemplateID = 4647722,
                TemplateLanguage = true,
                Subject = "Test Portal Notification",
                Variables = new Dictionary<string, object>
                {
                    {"TestUrl",context.Message.TestUrl },
                    {"AccessCode", r.AccessCode },
                },
            });
            var sendResult = await client.SendTransactionalEmailsAsync(mails);
            foreach (var item in sendResult.Messages.Where(c => c.Status != "success"))
            {
                logger.LogError("Failed to send test access code to email {0}", JsonSerializer.Serialize(item));
            }
        }
    }
}