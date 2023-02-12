using System;
using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels
{
    public class CreateOrUpdateTimeSettingsViewModel
    {
        public TestDuration TestDuration { get; set; } = default!;

        public TestActivationMethod TestActivationMethod { get; set; } = default!;
    }

    public class TimeSettings
    {
        public TestDuration TestDuration { get; set; } = default!;

        public TestActivationMethod TestActivationMethod { get; set; } = default!;
    }

    public class TestDuration
    {
        public TestDurationMethod Method { get; set; }

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
        public DateTimeOffset ActiveFromDate { get; set; }

        public DateTimeOffset ActiveUntilDate { get; set; }
    }

    public enum TestDurationMethod
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

