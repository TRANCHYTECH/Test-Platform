namespace VietGeeks.TestPlatform.AspNetCore;

public interface ITenant
{
    string UserId { get; }

    string Email { get; }
}