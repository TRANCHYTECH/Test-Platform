﻿using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Data.Models;

public class TestAccessSettingsPart
{
    public TestAcessType AccessType { get; set; }

    public TestAccessSettings Settings { get; set; } = default!;

    public HonestRespondentRule? HonestRespondentRule { get; set; }

    public static TestAccessSettingsPart Default()
    {
        return new TestAccessSettingsPart
        {
            AccessType = TestAcessType.PublicLink,
            Settings = new PublicLinkType
            {
                RequireAccessCode = false,
                Attempts = 10
            }
        };
    }
}

public enum TestAcessType
{
    PublicLink = 1,
    PrivateAccessCode = 2,
    GroupPassword = 3,
    Training = 4
}

[BsonKnownTypes(typeof(PublicLinkType), typeof(PrivateAccessCodeType), typeof(GroupPasswordType),
    typeof(TrainingType))]
public abstract class TestAccessSettings
{
}

public class PublicLinkType : TestAccessSettings
{
    public bool RequireAccessCode { get; set; }

    public int Attempts { get; set; }
}

public class PrivateAccessCodeType : TestAccessSettings
{
    public List<PrivateAccessCodeConfig> Configs { get; set; } = [];

    public int Attempts { get; set; }
}

public class PrivateAccessCodeConfig
{
    public string Code { get; set; } = default!;

    public string Email { get; set; } = default!;

    public string? SetId { get; set; }

    public bool SendCode { get; set; }
}

public class GroupPasswordType : TestAccessSettings
{
    public string Password { get; set; } = default!;

    public int Attempts { get; set; }
}

public class TrainingType : TestAccessSettings
{
}

public class HonestRespondentRule
{
    public bool Enable { get; set; }
}