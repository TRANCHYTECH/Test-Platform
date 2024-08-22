using Faker;
using FluentValidation;
using FluentValidation.TestHelper;
using MongoDB.Bson;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.QuestionDefinition;

namespace VietGeeks.TestPlatform.TestManager.UnitTest
{
    public static class UnitTestExtensions
    {
        public static void AssertError<T>(this TestValidationResult<T> validationResult, string forPropertyName,
            string withErrorMessage)
        {
            Assert.Equal(1,
                validationResult.Errors.Count(c =>
                    c.PropertyName == forPropertyName && c.ErrorCode == withErrorMessage));
        }
    }

    [Collection(QuestionDefinitionValidatorTestCollection.CollectionId)]
    public class QuestionDefinitionValidatorUnitTests
    {
        private readonly QuestionDefinitionValidatorFixture _fixture;
        private readonly QuestionDefinitionValidator _validator;

        public QuestionDefinitionValidatorUnitTests(QuestionDefinitionValidatorFixture fixture)
        {
            _fixture = fixture;
            _validator = _fixture.CreateQuestionDefinitionValidator();
        }

        //todo: validate each fields.
        [Fact]
        public async Task QuestionDefinition_Validate_Failure_Required_Fields()
        {
            var input = new QuestionDefinition();
            var ctx = new ValidationContext<QuestionDefinition>(input);
            var result = await _validator.TestValidateAsync(ctx);
            Assert.False(result.IsValid);
            result.AssertError(nameof(QuestionDefinition.TestId), "NotEmptyValidator");
            result.AssertError(nameof(QuestionDefinition.CategoryId), "NotEmptyValidator");
            result.AssertError(nameof(QuestionDefinition.Description), "NotEmptyValidator");
            result.AssertError(nameof(QuestionDefinition.AnswerType), "EnumValidator");
            result.AssertError(nameof(QuestionDefinition.Answers), "NotEmptyValidator");
            result.AssertError(nameof(QuestionDefinition.ScoreSettings), "NotNullValidator");
        }

        [Fact]
        public async Task QuestionDefinition_Validate_Failure_Business_Logics()
        {
            var input = new QuestionDefinition
            {
                TestId = "WrongTestId",
                CategoryId = "WrongTestCategoryId",
                Description = Lorem.Paragraph(3000),
                AnswerType = AnswerType.MultipleChoice,
                ScoreSettings = new SingleChoiceScoreSettings
                {
                    CorrectPoint = 10,
                    IncorrectPoint = 10,
                    TotalPoints = 10,
                    MustAnswerToContinue = true,
                    IsDisplayMaximumScore = true,
                    IsMandatory = true
                },
                Answers =
                [
                    new Answer(),
                    new Answer
                    {
                        AnswerDescription = "",
                        AnswerPoint = 0
                    }
                ]
            };
            var ctx = new ValidationContext<QuestionDefinition>(input);
            var result = await _validator.TestValidateAsync(ctx);
            Assert.False(result.IsValid);
            result.AssertError(nameof(QuestionDefinition.TestId), "ERR.QUESTIONDEF.TESTSID.NOTFOUND");
            result.AssertError(nameof(QuestionDefinition.CategoryId), "ERR.QUESTIONDEF.TESTCATID.NOTFOUND");
            result.AssertError(nameof(QuestionDefinition.Description), "MaximumLengthValidator");
            result.AssertError(nameof(QuestionDefinition.ScoreSettings), "ERR.QUESTIONDEF.SCORE.MISMATCH");
        }

        [Theory]
        [MemberData(nameof(CreateValidQuestionDefinition))]
        public async Task QuestionDefinition_Validate_Success(QuestionDefinition input)
        {
            var ctx = new ValidationContext<QuestionDefinition>(input);
            var result = await _validator.TestValidateAsync(ctx);
            Assert.True(result.IsValid);
        }

        public static IEnumerable<object[]> CreateValidQuestionDefinition()
        {
            var right1 = new QuestionDefinition
            {
                TestId = QuestionDefinitionValidatorFixture.QuestionMock.TestId,
                CategoryId = QuestionDefinitionValidatorFixture.QuestionMock.TestCategoryId,
                Description = "hardest question ever?",
                AnswerType = AnswerType.SingleChoice,
                ScoreSettings = new SingleChoiceScoreSettings(),
                Answers =
                [
                    new Answer
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        AnswerDescription = "Yes it's hardest",
                        AnswerPoint = 10
                    }
                ]
            };

            var right2 = new QuestionDefinition
            {
                TestId = QuestionDefinitionValidatorFixture.QuestionMock.TestId,
                CategoryId = QuestionDefinitionValidatorFixture.QuestionMock.TestCategoryId,
                Description = "hardest question ever?",
                AnswerType = AnswerType.MultipleChoice,
                ScoreSettings = new MultipleChoiceScoreSettings(),
                Answers =
                [
                    new Answer
                    {
                        Id = ObjectId.GenerateNewId().ToString(),
                        AnswerDescription = "Yes it's hardest",
                        AnswerPoint = 10
                    }
                ]
            };

            return new List<object[]>
            {
                new[] { right1 },
                new[] { right2 }
            };
        }
    }
}