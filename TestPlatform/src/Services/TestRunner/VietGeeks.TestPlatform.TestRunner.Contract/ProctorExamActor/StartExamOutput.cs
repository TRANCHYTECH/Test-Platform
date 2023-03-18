using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class StartExamOutput
{
    public ExamQuestion[] Questions { get; set; } = default!;

    public DateTime StartedAt { get; set; }
}