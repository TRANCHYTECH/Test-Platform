using System;
using System.Collections.Generic;
using System.Linq;
using ListShuffle;
using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Core.Models;

public class TestSetSettingsPart
{
    public TestSetGeneratorType GeneratorType { get; set; }

    public TestSetGenerator Generator { get; set; } = new DefaultGenerator();
}

public enum TestSetGeneratorType
{
    Default = 1,
    RandomByCategories = 2
}

[BsonKnownTypes(typeof(DefaultGenerator), typeof(RandomFromCategoriesGenerator))]
public abstract class TestSetGenerator
{
    public abstract List<QuestionDefinition> Generate(List<QuestionDefinition> questions);
}

/// <summary>
/// Default test set generator.
/// </summary>
public class DefaultGenerator : TestSetGenerator
{
    public override List<QuestionDefinition> Generate(List<QuestionDefinition> questions)
    {
        return questions;
    }
}

/// <summary>
/// Generate randomly by each category.
/// </summary>
public class RandomFromCategoriesGenerator : TestSetGenerator
{
    public List<RandomFromCategoriesGeneratorConfig> Configs { get; set; } = new List<RandomFromCategoriesGeneratorConfig>();

    public override List<QuestionDefinition> Generate(List<QuestionDefinition> questions)
    {
        var result = new List<QuestionDefinition>();
        foreach (var questionsByCategory in questions.GroupBy(c => c.CategoryId))
        {
            var radomizedQuestions = questionsByCategory.ToList();
            radomizedQuestions.Shuffle();

            var foundConfig = Configs.FirstOrDefault(c => c.QuestionCategoryId == questionsByCategory.Key);
            if (foundConfig != null)
            {
                result.AddRange(radomizedQuestions.GetRange(0, Math.Max(radomizedQuestions.Count, foundConfig.DrawNumber)));
            }
        }

        return result;
    }
}

public class RandomFromCategoriesGeneratorConfig
{
    [BsonElement("Id")]
    public string QuestionCategoryId { get; set; } = default!;

    [BsonElement("Draw")]
    public int DrawNumber { get; set; }
}

