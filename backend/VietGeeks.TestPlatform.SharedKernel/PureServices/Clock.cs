namespace VietGeeks.TestPlatform.SharedKernel.PureServices;

public class Clock : IClock
{
    public DateTime UtcNow => DateTime.UtcNow;
    public DateTime Now => DateTime.Now;
}

public interface IClock
{
    DateTime UtcNow { get; }
    DateTime Now { get; }
}