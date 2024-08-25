using Microsoft.Extensions.Logging;

namespace VietGeeks.TestPlatform.AccountManager
{

    public static partial class Logs
    {
        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Information,
            Message = "Created user with id=`{id}` username `{username}`")]
        public static partial void CreatedUser(this ILogger logger, string id, string username);

        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Information,
            Message = "Updated user with id=`{id}` username `{username}`")]
        public static partial void UpdatedUser(this ILogger logger, string id, string username);

        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Information,
            Message = "Created user expertise with id=`{id}` for user {userId}")]
        public static partial void CreatedUserExpertise(this ILogger logger, string id, string userId);

        [LoggerMessage(
            EventId = 0,
            Level = LogLevel.Information,
            Message = "Updated user expertise with id=`{id}` for user {userId}")]
        public static partial void UpdatedUserExpertise(this ILogger logger, string id, string userId);

        [LoggerMessage(
            EventId = 1,
            Level = LogLevel.Information,
            Message = "Received auth0 action: Create User. UserId: {userId}.Email: {email}")]
        public static partial void ReceivedCreatingUserAction(this ILogger logger, string userId, string email);


        [LoggerMessage(
            EventId = 1100,
            Level = LogLevel.Error,
            Message = "Could not create user. Duplidated User Id or Email")]
        public static partial void DuplicatedUser(this ILogger logger, Exception exception);
    }

}