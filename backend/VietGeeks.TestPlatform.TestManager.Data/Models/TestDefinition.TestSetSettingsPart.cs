using ListShuffle;
using MongoDB.Bson.Serialization.Attributes;

namespace VietGeeks.TestPlatform.TestManager.Data.Models
{
    public class TestSetSettingsPart
    {
        public TestSetGeneratorType GeneratorType { get; set; } = TestSetGeneratorType.Default;

        public TestSetGenerator Generator { get; set; } = new DefaultGenerator();

        public static TestSetSettingsPart Default()
        {
            return new TestSetSettingsPart
            {
                GeneratorType = TestSetGeneratorType.RandomByCategories,
                Generator = new RandomFromCategoriesGenerator()
            };
        }
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
    ///     Default test set generator.
    /// </summary>
    public class DefaultGenerator : TestSetGenerator
    {
        public override List<QuestionDefinition> Generate(List<QuestionDefinition> questions)
        {
            return questions;
        }
    }

//todo: move it to file logics.cs
/// <summary>
///     Generate randomly by each category.
/// </summary>
public class RandomFromCategoriesGenerator : TestSetGenerator
    {
        public List<RandomFromCategoriesGeneratorConfig> Configs { get; set; } = [];

        public override List<QuestionDefinition> Generate(List<QuestionDefinition> questions)
        {
            var result = new List<QuestionDefinition>();
            foreach (var questionsByCategory in questions.GroupBy(c => c.CategoryId))
            {
                var radomizedQuestions = questionsByCategory.ToList();
                radomizedQuestions.Shuffle();

                var matchedConfig = Configs.FirstOrDefault(c => c.QuestionCategoryId == questionsByCategory.Key);
                // When user selected this generator, but there is no specified config or wrong configured value. We should get best value.
                var questionCount = matchedConfig != null
                    ? Math.Max(radomizedQuestions.Count, matchedConfig.DrawNumber)
                    : radomizedQuestions.Count;
                result.AddRange(radomizedQuestions.GetRange(0, questionCount));
            }

            return result;
        }
    }

    public class RandomFromCategoriesGeneratorConfig
    {
        [BsonElement("Id")] public string QuestionCategoryId { get; set; } = default!;

        [BsonElement("Draw")] public int DrawNumber { get; set; }
    }
}