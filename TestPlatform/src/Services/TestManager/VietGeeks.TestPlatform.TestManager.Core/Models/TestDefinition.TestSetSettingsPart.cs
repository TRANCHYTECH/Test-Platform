using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class TestSetSettingsPart
{
    public TestSetGeneratorType GeneratorType { get; set; }

    public TestSetGenerator? Generator { get; set; }
}

public enum TestSetGeneratorType
{
    Default = 1,
    RandomByCategories = 2
}

[BsonKnownTypes(typeof(RandomFromCategoriesGenerator))]
public abstract class TestSetGenerator
{
}

/// <summary>
/// Generate randomly by each category.
/// </summary>
public class RandomFromCategoriesGenerator : TestSetGenerator
{
    public List<RandomFromCategoriesGeneratorConfig> Configs { get; set; } = new List<RandomFromCategoriesGeneratorConfig>();
}

public class RandomFromCategoriesGeneratorConfig
{
    [BsonElement("Id")]
    public string QuestionCategoryId { get; set; } = default!;

    [BsonElement("Draw")]
    public int DrawNumber { get; set; }
}

