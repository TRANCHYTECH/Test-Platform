using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

[Collection("TestDefinition")]
public class TestDefinition: EntityBase
{
    public TestBasicSettingsPart BasicSettings { get; set; } = default!;

    public TimeSettingsPart? TimeSettings { get; set; }

    public TestSetSettingsPart? TestSetSettings { get; set; }

    public GradingSettingsPart? GradingSettings { get; set; }

    public TestAccessSettingsPart? TestAccessSettings { get; set; }

    public TestStartSettingsPart? TestStartSettings { get; set; }

    public bool IsActivated { get; set; }

    public bool IsEnabled { get; set; }
}

