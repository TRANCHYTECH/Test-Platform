using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public interface IQuestionManagerService
{
    Task<IEnumerable<QuestionViewModel>> GetQuestions(string testId, CancellationToken cancellationToken);
    Task<PagedSearchResult<QuestionViewModel>> GetQuestions(string testId, int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<QuestionViewModel> GetQuestion(string id, CancellationToken cancellationToken);
    Task<QuestionViewModel> UpdateQuestion(string id, CreateOrUpdateQuestionViewModel questionViewModel, CancellationToken cancellationToken);
    Task<QuestionViewModel> CreateQuestion(string testId, CreateOrUpdateQuestionViewModel questionViewModel, CancellationToken cancellationToken);
    Task<IEnumerable<QuestionSummaryViewModel>> GetQuestionSummary(string testId, CancellationToken cancellationToken);
    Task<int> GetTotalPoints(string testId, CancellationToken cancellationToken);
    Task DeleteQuestion(string id, CancellationToken cancellationToken);
    Task UpdateQuestionOrders(string testId, UpdateQuestionOrderViewModel[] viewModel, CancellationToken cancellation);
}