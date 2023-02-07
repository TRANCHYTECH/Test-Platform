using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

public class TestAccessSettingsViewModel
{
    public TestAcessChannelViewModel Channel { get; set; }

    public TestAccessTypeViewModel AccessType { get; set; } = default!;

    public HonestRespondentRuleViewModel? HonestRespondentRule { get; set; }
}

public class CreateOrUpdateTestAccessSettingsViewModel
{
    public TestAcessChannelViewModel Channel { get; set; }

    public TestAccessTypeViewModel AccessType { get; set; } = default!;

    public HonestRespondentRuleViewModel? HonestRespondentRule { get; set; }
}

public enum TestAcessChannelViewModel
{
    WebBrowser = 1
}

[JsonDerivedType(typeof(PublicLinkTypeViewModel), 1)]
[JsonDerivedType(typeof(PrivateAccessCodeTypeViewModel), 2)]
[JsonDerivedType(typeof(GroupPasswordTypeViewModel), 3)]
[JsonDerivedType(typeof(TrainingTypeViewModel), 4)]
public abstract class TestAccessTypeViewModel
{
}

public class PublicLinkTypeViewModel : TestAccessTypeViewModel
{
    public bool RequireAccessCode { get; set; }

    public int AttemptsPerRespondent { get; set; }
}

public class PrivateAccessCodeTypeViewModel : TestAccessTypeViewModel
{
    public List<PrivateAccessCodeConfigViewModel> Configs { get; set; } = new List<PrivateAccessCodeConfigViewModel>();
}

public class PrivateAccessCodeConfigViewModel
{
    public string Code { get; set; } = default!;

    public string? Email { get; set; }

    public string? TestSetId { get; set; }

    public int AttemptsPerRespondent { get; set; }
}

public class GroupPasswordTypeViewModel : TestAccessTypeViewModel
{
    public string Password { get; set; } = default!;

    public int AttemptsPerRespondent { get; set; }
}

public class TrainingTypeViewModel : TestAccessTypeViewModel
{
}

public class HonestRespondentRuleViewModel
{
    public bool Enable { get; set; }
}
