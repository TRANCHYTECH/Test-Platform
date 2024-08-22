namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels
{
    public class TestDefinitionOverview
    {
        public string Id { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime CreatedOn { get; set; } = default!;
        public TestDefinitionStatus Status { get; set; } = default!;
        public string Category { get; set; } = default!;
    }
}