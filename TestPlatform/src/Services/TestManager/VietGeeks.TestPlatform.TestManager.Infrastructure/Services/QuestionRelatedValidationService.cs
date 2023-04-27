using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class QuestionRelatedValidationService : IQuestionRelatedValidationService
{
    private readonly TestManagerDbContext _managerDbContext;

    public QuestionRelatedValidationService(TestManagerDbContext managerDbContext)
    {
        _managerDbContext = managerDbContext;
    }

    public async Task<bool> CheckTestDefinitionExistence(string testId)
    {
        return await _managerDbContext.Find<TestDefinition>().MatchID(testId).ExecuteAnyAsync();
    }

    public async Task<bool> CheckTestCategoryExistence(string testCategoryId)
    {
        return testCategoryId == TestCategory.UncategorizedId || await _managerDbContext.Find<TestCategory>().MatchID(testCategoryId).ExecuteAnyAsync();
    }
}
