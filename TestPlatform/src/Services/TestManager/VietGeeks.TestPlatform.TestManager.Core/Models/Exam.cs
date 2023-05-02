﻿using System;
using System.Collections.Generic;
using MongoDB.Entities;
using VietGeeks.TestPlatform.TestManager.Core.ReadonlyModels;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("Exam")]
public class Exam : EntityBase
{
    public string TestRunId { get; set; } = default!;

    public string AccessCode { get; set; } = default!;

    public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
    
    public string[] Questions { get; set; } = default!;

    public Dictionary<string, string[]> Answers { get; set; } = default!;
    public Dictionary<string, int> QuestionScores { get; set; } = default!;

    public string? CurrentQuestionId { get; set; }

    public int FinalMark { get; set; }

    public List<AggregatedGrading>? Grading { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }
    public TimeSpan TotalTime { get; set; }
}


[Collection("TestRun")]
public class TestRun : EntityBase
{
    // As investigation, during test run, use could add, remove access code. code is only being used at step verifying.
    public TestDefinition TestDefinitionSnapshot { get; set; } = default!;

    public DateTime StartAtUtc { get; set; }

    public DateTime EndAtUtc { get; set; }

    public bool ExplicitEnd { get; set; } = false;
}


[Collection("TestRunQuestion")]
public class TestRunQuestion : EntityBase
{
    public string TestRunId { get; set; } = default!;

    public QuestionDefinition[] Batch { get; set; } = default!;
}
