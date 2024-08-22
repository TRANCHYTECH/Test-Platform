namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public interface IQuestionRelatedValidationService
{
    Task<bool> CheckTestCategoryExistence(string testCategoryId);
    Task<bool> CheckTestDefinitionExistence(string testId);
}