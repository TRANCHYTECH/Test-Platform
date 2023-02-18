﻿using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public interface IQuestionManagerService
    {
        Task<IEnumerable<QuestionViewModel>> GetQuestions(string testId, CancellationToken cancellationToken);
        Task<QuestionViewModel> GetQuestion(string id, CancellationToken cancellationToken);
        Task<QuestionViewModel> UpdateQuestion(string id, QuestionViewModel questionViewModel, CancellationToken cancellationToken);
        Task<QuestionViewModel> CreateQuestion(string testId, NewQuestionViewModel questionViewModel, CancellationToken cancellationToken);
    }
}