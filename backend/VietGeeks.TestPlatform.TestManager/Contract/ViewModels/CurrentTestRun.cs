namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

public class CurrentTestRun
{
    public string Id { get; set; } = default!;
    public DateTime ActivatedOrScheduledAtUtc { get; set; }
}