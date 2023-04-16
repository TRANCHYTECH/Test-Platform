using System;
namespace VietGeeks.TestPlatform.TestManager.WebApp.Models
{
    public class AppConfigurationSettings
    {
        public string TestManagerApiBaseUrl { get; set; } = default!;
        public string AccountManagerApiBaseUrl { get; set; } = default!;
        public string TestRunnerBaseUrl { get; set; } = default!;
        public AuthOptions Auth { get; set; } = new AuthOptions();
    }

    public class AuthOptions
    {
        public string Domain { get; set; } = default!;
        public string ClientId { get; set; } = default!;
        public string Audience { get; set; } = default!;
        public string Scope { get; set; } = default!;
        public string[] Intercepters { get; set; } = default!;

    }
}

