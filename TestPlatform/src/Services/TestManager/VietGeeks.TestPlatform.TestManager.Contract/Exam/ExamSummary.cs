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

public class ExamReview {
    
}
