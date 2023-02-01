using System;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("TestDefinition")]
public class TestDefinition: EntityBase
{
    public TestBasicSettingsPart BasicSettings { get; set; } = default!;

    public string InstructionMessage { get; set; } = default!;

    public TimeSettingsPart TimeSettings { get; set; } = default!;

    public bool IsSkipQuestionsEnabled { get; set; } = default!;

    public GradingSettings GradingSettings { get; set; } = default!;

    public bool IsActivated { get; set; }

    public bool IsEnabled { get; set; }
}

public class TestBasicSettingsPart
{
    public string Name { get; set; } = default!;

    public string Description { get; set; } = default!;

    public string Category { get; set; } = default!;
}


public class TimeSettingsPart
{
    public int MaximumTimeInMinutes { get; set; }

    public int TimePerQuestionInMinutes { get; set; }

    public DateTimeOffset ActiveFrom { get; set; }

    public DateTimeOffset ActiveTo { get; set; }
}

public class GradingSettings
{

}

