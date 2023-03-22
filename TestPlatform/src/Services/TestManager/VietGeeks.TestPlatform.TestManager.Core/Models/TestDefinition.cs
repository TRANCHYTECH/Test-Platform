﻿using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("TestDefinition")]
public class TestDefinition : EntityBase
{
    public TestBasicSettingsPart BasicSettings { get; set; } = default!;

    public TimeSettingsPart? TimeSettings { get; set; }

    public TestSetSettingsPart? TestSetSettings { get; set; }

    public GradingSettingsPart? GradingSettings { get; set; }

    public TestAccessSettingsPart? TestAccessSettings { get; set; }

    public TestStartSettingsPart? TestStartSettings { get; set; }

    public TestDefinitionStatus Status { get; private set; } = TestDefinitionStatus.Draft;

    //todo: replace by method check readiness for activation.
    public bool CanActivate => Status == TestDefinitionStatus.Draft && TimeSettings != null && TestSetSettings != null && GradingSettings != null && TestAccessSettings != null && TestStartSettings != null;

    public void Activate()
    {
        if (CanActivate)
        {
            Status = TestDefinitionStatus.Activated;
        }
    }
}

public enum TestDefinitionStatus
{
    Draft = 1,
    Activated = 2,
    Finished = 3,
    Deleted = 10
}
