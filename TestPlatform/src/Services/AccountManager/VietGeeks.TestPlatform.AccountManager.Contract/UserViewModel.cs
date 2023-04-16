using System;

namespace VietGeeks.TestPlatform.AccountManager.Contract
{
    public class UserViewModel
    {
        public string Email { get; set; } = default!;

        public RegionalSettingsViewModel? RegionalSettings { get; set; }
    }

    public class UserCreateViewModel
    {
        public string UserId { get; set; } = default!;

        public string Email { get; set; } = default!;
    }

    public class UserUpdateViewModel
    {
        public string? UserId { get; set; }

        public RegionalSettingsViewModel? RegionalSettings { get; set; }
    }

    public class RegionalSettingsViewModel
    {
        public string Language { get; set; } = default!;

        public string TimeZone { get; set; } = default!;
    }
}

