using Microsoft.Extensions.Logging;

namespace VietGeeks.TestPlatform.TestManager
{
    public static partial class Logs
    {
        [LoggerMessage(Level = LogLevel.Information, Message = "ReceivedE vent Sending Access Code")]
        public static partial void ReceivedEventSendingAccessCode(this ILogger logger);
    }
}