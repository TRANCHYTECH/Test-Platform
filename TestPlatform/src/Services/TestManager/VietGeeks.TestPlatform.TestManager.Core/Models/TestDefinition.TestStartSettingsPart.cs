using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestManager.Core.Models
{
    public class TestStartSettingsPart
    {
        public string? Instruction { get; set; }

        public string? Consent { get; set; }

        public List<RespondentIdentifyConfig> RespondentIdentifyConfig { get; set; } = default!;

        public static TestStartSettingsPart Default()
        {
            return new()
            {
                RespondentIdentifyConfig = new List<RespondentIdentifyConfig>
                {
                    new(){ FieldId = "FirstName", IsRequired = true },
                    new(){ FieldId = "LastName", IsRequired = true }
                }
            };
        }
    }

    public class RespondentIdentifyConfig
    {
        public string FieldId { get; set; } = default!;

        public bool IsRequired { get; set; }
    }
}

