namespace VietGeeks.TestPlatform.AccountManager.Data;

public class User : EntityBase
{
    public string Email { get; set; } = default!;

    public RegionalSettings RegionalSettings { get; set; } = default!;
}

public class RegionalSettings
{
    public string Language { get; set; } = default!;

    public string TimeZone { get; set; } = default!;
}