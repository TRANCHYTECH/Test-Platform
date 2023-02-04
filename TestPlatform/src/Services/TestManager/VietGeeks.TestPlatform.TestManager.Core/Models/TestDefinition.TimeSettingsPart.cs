using System;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class TimeSettingsPart
{
    public int MaximumTimeInMinutes { get; set; }

    public int TimePerQuestionInMinutes { get; set; }

    public DateTimeOffset ActiveFrom { get; set; }

    public DateTimeOffset ActiveTo { get; set; }
}

