using System;
namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public class Time : ITime
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}

