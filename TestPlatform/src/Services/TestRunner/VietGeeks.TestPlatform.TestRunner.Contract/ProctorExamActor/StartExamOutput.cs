using System;
using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class StartExamOutput: IActiveQuestion
{
    public ExamQuestion[] Questions { get; set; } = default!;

    public ExamQuestion? ActiveQuestion { get; set; } = default!;
    public int? ActiveQuestionIndex { get; set; }

    public TestDuration TestDuration { get; set; } = default!;
    public int TotalQuestion { get;set; } = default!;

    public DateTime StartedAt { get; set; }

    [JsonIgnore]
    public TimeSpan TotalDuration => TestDuration.Method == TestDurationMethodType.CompleteTestTime ? TestDuration.Duration : Questions.Length * TestDuration.Duration;
}

public class StartExamOutputViewModel : StartExamOutput, IExamStepInfo
{
    public ExamStep Step { get;set; }

}