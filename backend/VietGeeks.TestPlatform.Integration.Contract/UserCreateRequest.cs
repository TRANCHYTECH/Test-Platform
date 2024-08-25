using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.Integration.Contract;

public class UserCreateRequest
{
    public required string UserId { get; init; }

    public required string Email { get; init; }

    public (string ProviderId, string UserId) ParseUserId()
    {
        var parts = UserId.Split('|');

        if (parts.Length != 2 || !string.Equals(parts[0], "auth0", StringComparison.Ordinal))
        {
            throw new TestPlatformException($"UserId is invalid: {UserId}");
        }

        return (parts[0], parts[1]);
    }
}