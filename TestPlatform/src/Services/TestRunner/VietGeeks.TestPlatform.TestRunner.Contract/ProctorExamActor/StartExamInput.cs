﻿using System;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class StartExamInput
{
    public string ExamId { get; set; } = default!;
    public string TestRunId { get; set; } = default!;
}