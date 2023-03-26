using System;
namespace VietGeeks.TestPlatform.TestManager.Core.Models
{
    public class CurrentTestRunPart
    {
        public string Id { get; set; } = default!;

        public DateTime ActivatedOrScheduledAtUtc { get; set; }
    }
}

