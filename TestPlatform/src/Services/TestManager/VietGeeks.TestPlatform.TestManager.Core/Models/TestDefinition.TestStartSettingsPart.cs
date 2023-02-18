using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestManager.Core.Models
{
    public class TestStartSettingsPart
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

