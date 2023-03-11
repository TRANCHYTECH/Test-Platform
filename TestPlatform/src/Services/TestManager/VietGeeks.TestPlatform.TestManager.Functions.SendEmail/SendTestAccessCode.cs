﻿using System;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using VietGeeks.TestPlatform.Integration.Contracts;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace VietGeeks.TestPlatform.TestManager.Functions.SendEmail;

public class SendTestAccessCode
{
    private readonly ILogger _logger;

    public SendTestAccessCode(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<SendTestAccessCode>();
    }

    [Function("SendTestAccessCode")]
    public async Task Run([ServiceBusTrigger("send-test-access-code", Connection = "TestRunnerSB")] SendTestAccessCodeRequest invitation)
    {
        //todo: move harded code settings 
        MailjetClient client = new(Environment.GetEnvironmentVariable("MJ_APIKEY_PUBLIC"), Environment.GetEnvironmentVariable("MJ_APIKEY_PRIVATE"));
        var mails = invitation.Receivers.Select(r => new TransactionalEmail
        {
            From = new("notify@testmaster.io", "Test Master Notification"),
            To = new List<SendContact>
            {
                new(r.Email)
            },
            TemplateID = 4647722,
            TemplateLanguage = true,
            Subject = "Test Master Notification",
            Variables = new Dictionary<string, object>
            {
                {"TestUrl",invitation.TestUrl },
                {"AccessCode", r.AccessCode }
            }
        });
        var sendResult = await client.SendTransactionalEmailsAsync(mails);
        foreach (var item in sendResult.Messages.Where(c=>c.Status != "success"))
        {
            _logger.LogError("Failed to send test access code to email {0}", item.To[0]);
        }
    }
}
