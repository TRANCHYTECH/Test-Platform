namespace VietGeeks.TestPlatform.AspNetCore.Services
{
    public interface ITenant
    {
        string UserId { get; }

        string Email { get; }
    }
}