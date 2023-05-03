using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public interface IQuestionCategoryService
    {
        Task<QuestionCategoryViewModel> CreateCategory(string testId, NewQuestionCategoryViewModel questionCategory, CancellationToken cancellationToken);
        Task<IEnumerable<QuestionCategoryViewModel>> GetCategories(string testId, CancellationToken cancellationToken);
        Task DeleteCategories(string[] ids);
    }
}
