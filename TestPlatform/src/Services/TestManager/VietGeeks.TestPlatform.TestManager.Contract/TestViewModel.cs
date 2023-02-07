using System;
using System.Collections.Generic;
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
    public List<RandomByCategoriesGeneratorConfigViewModel> Configs { get; set; } = new List<RandomByCategoriesGeneratorConfigViewModel>();
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



