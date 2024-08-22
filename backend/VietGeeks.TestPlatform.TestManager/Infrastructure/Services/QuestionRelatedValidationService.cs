using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public class QuestionRelatedValidationService(TestManagerDbContext managerDbContext)
    : IQuestionRelatedValidationService
{
    public async Task<bool> CheckTestDefinitionExistence(string testId)
    {
        return await managerDbContext.Find<TestDefinition>().MatchID(testId).ExecuteAnyAsync();
    }

    public async Task<bool> CheckTestCategoryExistence(string testCategoryId)
    {
        return testCategoryId == CategoryBase.UncategorizedId ||
               await managerDbContext.Find<TestCategory>().MatchID(testCategoryId).ExecuteAnyAsync();
    }
}