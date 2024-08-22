using Moq;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.QuestionDefinition;

namespace VietGeeks.TestPlatform.TestManager.UnitTest
{
    public class QuestionDefinitionValidatorFixture : IDisposable
    {
        public static readonly (string TestId, string TestCategoryId) QuestionMock = ("107f191e810c19729de860ef",
            "107f191e810c19729de860ef");

        public Mock<IQuestionRelatedValidationService> QuestionRelatedValidationServiceMock = new();


        public QuestionDefinitionValidatorFixture()
        {
            QuestionRelatedValidationServiceMock.Setup(c => c.CheckTestDefinitionExistence(QuestionMock.TestId))
                .ReturnsAsync(true);
            QuestionRelatedValidationServiceMock.Setup(c => c.CheckTestCategoryExistence(QuestionMock.TestCategoryId))
                .ReturnsAsync(true);
        }

        public void Dispose()
        {
            //
        }

        public QuestionDefinitionValidator CreateQuestionDefinitionValidator()
        {
            return new QuestionDefinitionValidator(QuestionRelatedValidationServiceMock.Object, new AnswerValidator(),
                new SingleChoiceScoreSettingsValidator(), new MultipleChoiceScoreSettingsValidator());
        }
    }

    [CollectionDefinition(CollectionId)]
    public class QuestionDefinitionValidatorTestCollection : ICollectionFixture<QuestionDefinitionValidatorFixture>
    {
        public const string CollectionId = "QuestionDefinitionValidatorTestCollection";
    }
}