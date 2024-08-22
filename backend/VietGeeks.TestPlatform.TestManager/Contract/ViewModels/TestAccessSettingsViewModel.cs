using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

public class TestAccessSettingsViewModel
{
    public TestAccessType AccessType { get; set; }

    public TestAccessConfigViewModel Settings { get; set; } = default!;

    public HonestRespondentRuleViewModel? HonestRespondentRule { get; set; }
}

public class CreateOrUpdateTestAccessSettingsViewModel
{
    public TestAccessType AccessType { get; set; }

    public TestAccessConfigViewModel? Settings { get; set; } = default!;

    public HonestRespondentRuleViewModel? HonestRespondentRule { get; set; }
}

[JsonDerivedType(typeof(PublicLinkTypeViewModel), (int)TestAccessType.PublicLink)]
[JsonDerivedType(typeof(PrivateAccessCodeTypeViewModel), (int)TestAccessType.PrivateAccessCode)]
[JsonDerivedType(typeof(GroupPasswordTypeViewModel), (int)TestAccessType.GroupPassword)]
[JsonDerivedType(typeof(TrainingTypeViewModel), (int)TestAccessType.Training)]
public abstract class TestAccessConfigViewModel
{
}

public class PublicLinkTypeViewModel : TestAccessConfigViewModel
{
    public bool RequireAccessCode { get; set; }

    public int Attempts { get; set; }
}

public class PrivateAccessCodeTypeViewModel : TestAccessConfigViewModel
{
    public List<PrivateAccessCodeConfigViewModel> Configs { get; set; } = [];

    public int Attempts { get; set; }
}

public class PrivateAccessCodeConfigViewModel
{
    public string Code { get; set; } = default!;

    public string? SetId { get; set; }

    public string? Email { get; set; }

    public bool SendCode { get; set; }
}

public class GroupPasswordTypeViewModel : TestAccessConfigViewModel
{
    public string Password { get; set; } = default!;

    public int Attempts { get; set; }
}

public class TrainingTypeViewModel : TestAccessConfigViewModel
{
}

public class HonestRespondentRuleViewModel
{
    public bool Enable { get; set; }
}

public enum TestAccessType
{
    PublicLink = 1,
    PrivateAccessCode = 2,
    GroupPassword = 3,
    Training = 4
}