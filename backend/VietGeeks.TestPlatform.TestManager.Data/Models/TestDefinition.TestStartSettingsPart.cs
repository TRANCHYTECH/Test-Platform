namespace VietGeeks.TestPlatform.TestManager.Data.Models;

public class TestStartSettingsPart
{
    public string? Instruction { get; set; }

    public string? Consent { get; set; }

    public List<RespondentIdentifyConfig> RespondentIdentifyConfig { get; set; } = default!;

    public static TestStartSettingsPart Default()
    {
        return new TestStartSettingsPart
        {
            RespondentIdentifyConfig =
            [
                new RespondentIdentifyConfig { FieldId = "FirstName", IsRequired = true },
                new RespondentIdentifyConfig { FieldId = "LastName", IsRequired = true }
            ]
        };
    }
}

public class RespondentIdentifyConfig
{
    public string FieldId { get; set; } = default!;

    public bool IsRequired { get; set; }
}