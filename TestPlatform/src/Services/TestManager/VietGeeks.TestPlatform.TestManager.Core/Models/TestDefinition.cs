using MongoDB.Entities;

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

    public CurrentTestRunPart? CurrentTestRun { get; set; }

    public TestDefinitionStatus Status { get; private set; } = TestDefinitionStatus.Draft;

    public void Activate(string testRunId)
    {
        Status = TestDefinitionStatus.Activated;
        CurrentTestRun = new CurrentTestRunPart
        {
            Id = testRunId
        };
    }
}

public enum TestDefinitionStatus
{
    Draft = 1,
    Activated = 2,
    Scheduled = 3,
    Ended = 4,
    Deleted = 10
}
