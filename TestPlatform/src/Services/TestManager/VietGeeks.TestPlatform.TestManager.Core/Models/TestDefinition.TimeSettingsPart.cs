using System;
using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class TimeSettingsPart
{
    public TestDurationMethod TestDurationMethod { get; set; } = default!;

    public TestActivationMethod TestActivationMethod { get; set; } = default!;
}

[BsonKnownTypes(typeof(CompleteTestDuration), typeof(CompleteQuestionDuration))]
public abstract class TestDurationMethod
{
}

public class CompleteTestDuration : TestDurationMethod
{
    public TimeSpan Duration { get; set; }
}

public class CompleteQuestionDuration : TestDurationMethod
{
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
    public DateTime ActiveFromDate { get; set; }

    public DateTime ActiveUntilDate { get; set; }
}