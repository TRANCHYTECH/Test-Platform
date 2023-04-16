﻿using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class ExamStatus: IActiveQuestion
{
    public string ExamId { get; set; } = default!;
    public ExamQuestion? ActiveQuestion { get; set; }
    public int? ActiveQuestionIndex { get; set; }
    public DateTime? ActiveQuestionStartedAt {get;set;}
    public int? QuestionCount { get; set; }
    public TestDuration TestDuration { get; set; } = default!;
    
    public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
}

public class ExamStatusWithStep: ExamStatus, IExamStepInfo
{
    public ExamStep Step { get;set; }
}