namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels
{
    public class TestInvitationStatsInput : TestInvitationStatsViewModel
    {
        public string TestDefinitionId { get; set; } = default!;
    }

    public class TestInvitationStatsViewModel
    {
        public string TestRunId { get; set; } = default!;
        public string[] AccessCodes { get; set; } = default!;
    }
}