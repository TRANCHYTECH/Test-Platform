using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class TestSetSettingsPart
{
    public TestSetGeneratorTypes GeneratorType { get; set; }

    public ITestSetGenerator? Generator { get; set; }
}

public enum TestSetGeneratorTypes
{
    Default = 1,
    RandomByCategories = 2
}

public interface ITestSetGenerator
{
}

/// <summary>
/// Generate randomly by each category.
/// </summary>
public class RandomByCategoriesGenerator : ITestSetGenerator
{
    public List<RandomByCategoriesGeneratorConfig> Configs { get; set; } = new List<RandomByCategoriesGeneratorConfig>();
}

public class RandomByCategoriesGeneratorConfig
{
    public string QuestionCategoryId { get; set; } = default!;

    public int DrawNumber { get; set; }
}

