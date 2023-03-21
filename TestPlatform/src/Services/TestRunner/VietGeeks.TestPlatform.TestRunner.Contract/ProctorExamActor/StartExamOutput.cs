using System;
using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class StartExamOutput
{
    public ExamQuestion[] Questions { get; set; } = default!;

    public TestDuration TestDuration { get; set; } = default!;

    public DateTime StartedAt { get; set; }

    [JsonIgnore]
    public TimeSpan TotalDuration => TestDuration.Method == TestDurationMethodType.CompleteTestTime ? TestDuration.Duration : Questions.Length * TestDuration.Duration;
}