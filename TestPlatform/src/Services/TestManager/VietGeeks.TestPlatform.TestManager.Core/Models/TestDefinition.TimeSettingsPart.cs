using System;
using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class TimeSettingsPart
{
    public TestDuration TestDuration { get; set; } = default!;

    public TestActivationMethod TestActivationMethod { get; set; } = default!;
}

public class TestDuration
{
    public TestDurationMethod Method { get; set; }

    public TimeSpan Duration { get; set; }
}

[BsonKnownTypes(typeof(ManualTestActivation), typeof(TimePeriodActivation))]
public abstract class TestActivationMethod
{
}

public class ManualTestActivation : TestActivationMethod
{
    public TimeSpan ActiveUntil { get; set; }
}


public class TimePeriodActivation : TestActivationMethod
{
    public DateTimeOffset ActiveFromDate { get; set; }

    public DateTimeOffset ActiveUntilDate { get; set; }
}

public enum TestDurationMethod
{
    CompleteTestTime = 1,
    CompleteQuetsionTime = 2
}