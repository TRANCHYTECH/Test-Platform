using Microsoft.Extensions.Logging;

namespace VietGeeks.TestPlatform.TestManager;

public static partial class Logs
{
    [LoggerMessage(Level = LogLevel.Information, Message = "Received the event Sending Access Code")]
    public static partial void ReceivedEventSendingAccessCode(this ILogger logger);
}