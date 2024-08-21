using System.Text.Json.Serialization;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

namespace VietGeeks.TestPlatform.TestManager.Contract;

public class NewTestDefinitionViewModel
{
    public CreateOrUpdateTestBasicSettingsViewModel BasicSettings { get; set; } = default!;
}

public class UpdateTestDefinitionViewModel
{
    public CreateOrUpdateTestBasicSettingsViewModel? BasicSettings { get; set; }

    public CreateOrUpdateTestSetSettingsViewModel? TestSetSettings { get; set; }

    public CreateOrUpdateTestAccessSettingsViewModel? TestAccessSettings { get; set; }

    public CreateOrUpdateGradingSettings? GradingSettings { get; set; }

    public CreateOrUpdateTimeSettingsViewModel? TimeSettings { get; set; }

    public CreateOrUpdateTestStartSettingsViewModel? TestStartSettings { get; set; }
}

public class CreateOrUpdateTestBasicSettingsViewModel
{
    public string Name { get; set; } = default!;

    public string Category { get; set; } = default!;

    public string? Language { get; set; }

    public string Description { get; set; } = default!;
}

public class TestBasicSettingsViewModel
{
    public string Name { get; set; } = default!;

    public string Category { get; set; } = default!;

    public string? Language { get; set; }

    public string Description { get; set; } = default!;
}

public class TestDefinitionViewModel
{
    public string Id { get; set; } = default!;

    public TestBasicSettingsViewModel BasicSettings { get; set; } = default!;

    public TestSetSettingsViewModel? TestSetSettings { get; set; }

    public TestAccessSettingsViewModel? TestAccessSettings { get; set; }

    public GradingSettings? GradingSettings { get; set; }

    public TimeSettings? TimeSettings { get; set; }

    public TestStartSettingsViewModel? TestStartSettings { get; set; }

    public CurrentTestRun? CurrentTestRun { get; set; }

    public TestDefinitionStatus Status { get; set; }

    public DateTime CreatedOn { get; set; }

    public DateTime ModifiedOn { get; set; }
}

public class CreateOrUpdateTestSetSettingsViewModel
{
    public TestSetGeneratorType GeneratorType { get; set; }

    public TestSetGeneratorViewModel? Generator { get; set; }
}

public class TestSetSettingsViewModel
{
    public TestSetGeneratorType GeneratorType { get; set; }

    public TestSetGeneratorViewModel? Generator { get; set; }
}

[JsonDerivedType(typeof(DefaultGeneratorViewModel), 1)]
[JsonDerivedType(typeof(RandomByCategoriesGeneratorViewModel), 2)]
public abstract class TestSetGeneratorViewModel
{
}

public class DefaultGeneratorViewModel : TestSetGeneratorViewModel
{
}

public class RandomByCategoriesGeneratorViewModel : TestSetGeneratorViewModel
{
    public List<RandomByCategoriesGeneratorConfigViewModel> Configs { get; set; } = [];
}

public class RandomByCategoriesGeneratorConfigViewModel
{
    [JsonPropertyName("id")]
    public string QuestionCategoryId { get; set; } = default!;

    [JsonPropertyName("draw")]
    public int DrawNumber { get; set; }
}

public enum TestSetGeneratorType
{
    Default = 1,
    RandomByCategories = 2
}

public enum TestDefinitionStatus
{
    Draft = 1,
    Activated = 2,
    Finished = 3,
    Deleted = 10
}
