﻿namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class ExamStatus : IActiveQuestion
{
    public string ExamId { get; set; } = default!;
    public string TestName { get; set; } = default!;
    public DateTime? ActiveQuestionStartedAt { get; set; }
    public int? QuestionCount { get; set; }
    public TestDuration TestDuration { get; set; } = default!;
    public TimeSpan? TotalDuration { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime? FinishededAt { get; set; }
    public IDictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
    public IEnumerable<AggregatedGradingOuput> Grading { get; set; } = default!;
    public bool CanSkipQuestion { get; set; }
    public bool CanFinish { get; set; }
    public ExamQuestion? ActiveQuestion { get; set; }
    public int? ActiveQuestionIndex { get; set; }
}

public class ExamStatusWithStep : ExamStatus, IExamStepInfo
{
    public ExamStep Step { get; set; }
}