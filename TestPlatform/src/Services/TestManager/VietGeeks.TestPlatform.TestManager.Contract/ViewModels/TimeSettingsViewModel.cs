using System;
using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels
{
    public class CreateOrUpdateTimeSettingsViewModel
    {
        public TestDurationMethod TestDurationMethod { get; set; } = default!;

        public TestActivationMethod TestActivationMethod { get; set; } = default!;
    }

    public class TimeSettings
    {
        public TestDurationMethod TestDurationMethod { get; set; } = default!;

        public TestActivationMethod TestActivationMethod { get; set; } = default!;
    }


    [JsonDerivedType(typeof(CompleteTestDuration), (int)TestDurationMethodTypoe.CompleteTestTime)]
    [JsonDerivedType(typeof(CompleteQuestionDuration), (int)TestDurationMethodTypoe.CompleteQuetsionTime)]
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


    [JsonDerivedType(typeof(ManualTestActivation), (int)TestActivationMethodType.ManualTest)]
    [JsonDerivedType(typeof(TimePeriodActivation), (int)TestActivationMethodType.TimePeriod)]
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

    public enum TestDurationMethodTypoe
    {
        CompleteTestTime = 1,
        CompleteQuetsionTime = 2
    }

    public enum TestActivationMethodType
    {
        ManualTest = 1,
        TimePeriod = 2
    }
}

