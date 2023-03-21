using System;
using System.Collections.Generic;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("Exam")]
public class Exam : EntityBase
{
    public string TestId { get; set; } = default!;

    public string? AccessCode { get; set; }

    public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();

    public List<QuestionDefinition> Questions { get; set; } = default!;

    public int FinalPoints { get; set; }

    public DateTime StartedAt { get; set; }

    public DateTime FinishedAt { get; set; }

    public TestDurationMethod TestDurationMethod { get; set; } = default!;

    public TimeSettingsPart TimeSettings { get; set; } = default!;
}
