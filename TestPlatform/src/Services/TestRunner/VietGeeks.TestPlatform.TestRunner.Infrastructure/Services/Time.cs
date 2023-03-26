using System;
namespace VietGeeks.TestPlatform.TestRunner.Infrastructure.Services
{
    public class Time : ITime
    {
        public DateTime UtcNow()
        {
            return DateTime.UtcNow;
        }
    }
}

