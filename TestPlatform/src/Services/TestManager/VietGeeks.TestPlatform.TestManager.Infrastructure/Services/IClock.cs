namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public interface IClock
{
    DateTime UtcNow { get; }
}