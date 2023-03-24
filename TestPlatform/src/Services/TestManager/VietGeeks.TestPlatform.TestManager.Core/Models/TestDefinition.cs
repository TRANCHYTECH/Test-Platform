using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("TestDefinition")]
public class TestDefinition : EntityBase
{
    public TestBasicSettingsPart BasicSettings { get; set; } = default!;

    public TimeSettingsPart TimeSettings { get; set; } = default!;

    public TestSetSettingsPart TestSetSettings { get; set; } = default!;

    public GradingSettingsPart GradingSettings { get; set; } = default!;

    public TestAccessSettingsPart TestAccessSettings { get; set; } = default!;

    public TestStartSettingsPart TestStartSettings { get; set; } = default!;

    public CurrentTestRunPart? CurrentTestRun { get; set; }

    public TestDefinitionStatus Status { get; private set; } = TestDefinitionStatus.Draft;

    public void Activate(string testRunId, TestDefinitionStatus status)
    {
        if (status == TestDefinitionStatus.Activated || status == TestDefinitionStatus.Scheduled)
        {
            Status = status;
            CurrentTestRun = new CurrentTestRunPart
            {
                Id = testRunId
            };
        }
        else
        {
            throw new TestPlatformException("Invalid status");
        }

    }
}

public enum TestDefinitionStatus
{
    Draft = 1,
    Activated = 2,
    Scheduled = 3,
    Ended = 4
}
