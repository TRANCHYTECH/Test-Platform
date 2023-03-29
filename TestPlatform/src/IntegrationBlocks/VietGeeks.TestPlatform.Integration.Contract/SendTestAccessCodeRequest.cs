using System;

namespace VietGeeks.TestPlatform.Integration.Contracts;

public class SendTestAccessCodeRequest
{
    public string TestUrl { get; set; } = default!;

    public string TestDefinitionId { get; set; } = default!;

    public Receiver[] Receivers { get; set; } = default!;

    public string TestRunId { get; set; } = default!;

    public string GenerateReferenceId(string accessCode) {
        return $"{TestDefinitionId}_{TestRunId}_{accessCode}";
    }
}

public class Receiver
{
    public Receiver(string accessCode, string email)
    {
        AccessCode = accessCode;
        Email = email;
    }

    public string AccessCode { get; set; } = default!;

    public string Email { get; set; } = default!;
}

