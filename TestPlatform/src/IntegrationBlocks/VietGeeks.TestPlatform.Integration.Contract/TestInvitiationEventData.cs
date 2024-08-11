using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.Integration.Contract;

public class TestInvitationEventData
{
    public List<Dictionary<string, string>> Events { get; set; } = new List<Dictionary<string, string>>();
}

