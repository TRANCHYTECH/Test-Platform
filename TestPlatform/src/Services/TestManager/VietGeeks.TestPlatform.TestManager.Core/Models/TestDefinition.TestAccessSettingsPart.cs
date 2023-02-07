using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class TestAccessSettingsPart
{
    public TestAcessChannel Channel { get; set; }

    public TestAccessType? AccessType { get; set; }

    public HonestRespondentRule? HonestRespondentRule { get; set; }
}

public enum TestAcessChannel
{
    WebBrowser = 1
}

[BsonKnownTypes(typeof(PublicLinkType), typeof(PrivateAccessCodeType), typeof(GroupPasswordType), typeof(TrainingType))]
public abstract class TestAccessType
{
}

public class PublicLinkType : TestAccessType
{
    public bool RequireAccessCode { get; set; }

    public int AttemptsPerRespondent { get; set; }
}

public class PrivateAccessCodeType : TestAccessType
{
}

public class PrivateAccessCodeConfig
{
    public string Code { get; set; } = default!;    

    public string? Email { get; set; }

    public string? TestSetId { get; set; }

    public int AttemptsPerRespondent { get; set; }
}

public class GroupPasswordType: TestAccessType
{
    public string Password { get; set; } = default!;

    public int AttemptsPerRespondent { get; set; }
}

public class TrainingType : TestAccessType
{
}

public class HonestRespondentRule
{
    public bool Enable { get; set; }
}