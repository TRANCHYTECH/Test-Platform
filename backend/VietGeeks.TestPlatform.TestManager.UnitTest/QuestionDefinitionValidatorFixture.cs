using Moq;
using VietGeeks.TestPlatform.TestManager.Infrastructure;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.QuestionDefinition;

namespace VietGeeks.TestPlatform.TestManager.UnitTest;

public class QuestionDefinitionValidatorFixture : IDisposable
{
    public Mock<IQuestionRelatedValidationService> QuestionRelatedValidationServiceMock = new();
    public static readonly (string TestId, string TestCategoryId) QuestionMock = ("107f191e810c19729de860ef", "107f191e810c19729de860ef");


    public QuestionDefinitionValidatorFixture()
    {
        QuestionRelatedValidationServiceMock.Setup(c => c.CheckTestDefinitionExistence(QuestionMock.TestId)).ReturnsAsync(true);
        QuestionRelatedValidationServiceMock.Setup(c => c.CheckTestCategoryExistence(QuestionMock.TestCategoryId)).ReturnsAsync(true);
    }

    public QuestionDefinitionValidator CreateQuestionDefinitionValidator()
    {
        return new QuestionDefinitionValidator(QuestionRelatedValidationServiceMock.Object, new AnswerValidator(), new SingleChoiceScoreSettingsValidator(), new MultipleChoiceScoreSettingsValidator());
    }

    public void Dispose()
    {
        //
    }
}

[CollectionDefinition(CollectionId)]
public class QuestionDefinitionValidatorTestCollection : ICollectionFixture<QuestionDefinitionValidatorFixture>
{
    public const string CollectionId = "QuestionDefinitionValidatorTestCollection";
}

