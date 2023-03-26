using System;
namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public class Clock : IClock
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

