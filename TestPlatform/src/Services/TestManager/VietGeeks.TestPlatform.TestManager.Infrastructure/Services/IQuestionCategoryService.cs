using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public interface IQuestionCategoryService
    {
        Task<string> CreateQuestionCategory(NewQuestionCategoryViewModel questionCategory, CancellationToken cancellationToken);
        Task<IEnumerable<QuestionCategoryViewModel>> GetCategories(CancellationToken cancellationToken);
    }
}
