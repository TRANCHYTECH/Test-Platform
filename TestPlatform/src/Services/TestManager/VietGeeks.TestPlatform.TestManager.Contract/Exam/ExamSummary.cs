using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestManager.Contract.Exam;
public class ExamSummary
{
    public string Id { get; set; } = default!;

    public int FinalMark { get; set; }

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public Dictionary<string, string> ExamineeInfo { get; set; } = default!;

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }

    public string TotalTime { get; set; } = default!;
}

public class ExamReview
{
    public IEnumerable<dynamic> Questions { get; set; } = default!;

    public Dictionary<string, string[]> Answers { get; set; } = default!;

    public string FirstName { get; set; } = default!;

    public string LastName { get; set; } = default!;

    public dynamic Scores { get; set; } = default!;

    public dynamic? Grading { get; set; } = default!;

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }

    public string ActualTotalDuration { get; set; } = default!;

    public string ConfiguredTotalTime { get; set; } = default!;

    public string TotalDuration { get; set; } = default!;
}
