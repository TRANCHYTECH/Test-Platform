using Moq;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefintion;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.UnitTest;

public class TestDefinitionValidatorFixture : IDisposable
{
    public Mock<ITestCategoryService> TestCategoryServiceMock = new();
    public Mock<IQuestionManagerService> QuestionManagerServiceMock = new();
    public Mock<IClock> ClockMock = new();
    public static readonly (string Id, int TotalPoints) TestMock = ("107f191e810c19729de860ef", 30);

    public TestDefinitionValidatorFixture()
    {
        TestCategoryServiceMock.Setup(c => c.CheckTestCategoryExistence(It.IsAny<string>())).ReturnsAsync((string arg) => arg == "yesme" ? true : false);
        QuestionManagerServiceMock.Setup(c => c.GetTotalPoints(TestMock.Id, default)).ReturnsAsync(TestMock.TotalPoints);
        ClockMock.Setup(c => c.UtcNow).Returns(DateTime.UtcNow);
    }

    public TestBasicSettingsPartValidator CreateTestBasicSettingsPartValidator()
    {
        return new TestBasicSettingsPartValidator(TestCategoryServiceMock.Object);
    }

    public TestSetSettingsPartValidator CreateTestSetSettingsPartValidator()
    {
        return new TestSetSettingsPartValidator(new DefaultGeneratorValidator(), new RandomFromCategoriesGeneratorValidator(new RandomFromCategoriesGeneratorConfigValidator()));
    }

    public TestAccessSettingsPartValidator CreateTestAccessSettingsPartValidator()
    {
        return new TestAccessSettingsPartValidator(new PublicLinkTypeValidator(), new PrivateAccessCodeTypeValidator(new PrivateAccessCodeConfigValidator()), new GroupPasswordTypeValidator(), new TrainingTypeValidator());
    }

    public GradingSettingsPartValidator CreateGradingSettingsPartValidator()
    {
        return new GradingSettingsPartValidator(new TestEndConfigValidator(), new PassMaskCriteriaValidator(), new GradeRangeCriteriaValidator(QuestionManagerServiceMock.Object), new InformRespondentConfigValidator());
    }

    public TestDefinitionValidator CreateTestDefinitionValidator()
    {
        return new TestDefinitionValidator(CreateTestBasicSettingsPartValidator(), CreateTestSetSettingsPartValidator(), CreateTestAccessSettingsPartValidator(), CreateGradingSettingsPartValidator());
    }

    public void Dispose()
    {
    }

    public TimeSettingsPartValidator CreateTimeSettingsPartValidator()
    {
        return new TimeSettingsPartValidator(
            new CompleteQuestionDurationValidator(),
            new CompleteTestDurationValidator(),
            new ManualTestActivationValidator(),
            new TimePeriodActivationValidator(ClockMock.Object),
            new AnswerQuestionConfigValidator());
    }
}
