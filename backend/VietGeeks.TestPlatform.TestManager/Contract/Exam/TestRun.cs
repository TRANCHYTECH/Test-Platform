namespace VietGeeks.TestPlatform.TestManager.Contract.Exam;

public class TestRunSummary
{
    public string Id { get; set; } = default!;

    public DateTime StartAt { get; set; }

    public DateTime EndAt { get; set; }
}