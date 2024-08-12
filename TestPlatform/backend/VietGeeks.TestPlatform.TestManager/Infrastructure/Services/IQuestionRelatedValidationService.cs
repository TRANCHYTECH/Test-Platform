namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public interface IQuestionRelatedValidationService
{
    Task<bool> CheckTestCategoryExistence(string testCategoryId);
    Task<bool> CheckTestDefinitionExistence(string testId);
}