using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Data.Models
{
    [Collection("TestDefinition")]
    public partial class TestDefinition : EntityBase
    {
        public TestBasicSettingsPart BasicSettings { get; set; } = default!;

        public TimeSettingsPart TimeSettings { get; set; } = default!;

        public TestSetSettingsPart TestSetSettings { get; set; } = default!;

        public GradingSettingsPart GradingSettings { get; set; } = default!;

        public TestAccessSettingsPart TestAccessSettings { get; set; } = default!;

        public TestStartSettingsPart TestStartSettings { get; set; } = default!;

        public CurrentTestRunPart? CurrentTestRun { get; private set; }

        public TestDefinitionStatus Status { get; private set; } = TestDefinitionStatus.Draft;
    }

    public enum TestDefinitionStatus
    {
        Draft = 1,
        Activated = 2,
        Scheduled = 3,
        Ended = 4
    }
}