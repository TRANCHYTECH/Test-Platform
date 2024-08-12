namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels
{
    public class CreateOrUpdateTestStartSettingsViewModel
    {
        public string? Instruction { get; set; }

        public string? Consent { get; set; }

        public List<RespondentIdentifyConfig> RespondentIdentifyConfig { get; set; } = default!;
    }

    public class TestStartSettingsViewModel
    {
        public string? Instruction { get; set; }

        public string? Consent { get; set; }

        public List<RespondentIdentifyConfig> RespondentIdentifyConfig { get; set; } = default!;
    }

    public class RespondentIdentifyConfig
    {
        public string FieldId { get; set; } = default!;

        public bool IsRequired { get; set; }
    }
}

