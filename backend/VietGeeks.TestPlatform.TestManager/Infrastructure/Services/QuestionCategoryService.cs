using AutoMapper;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public class QuestionCategoryService(IMapper mapper, TestManagerDbContext managerDbContext)
    : IQuestionCategoryService
{
    public async Task<QuestionCategoryViewModel> CreateCategory(string testId,
        NewQuestionCategoryViewModel questionCategory, CancellationToken cancellationToken)
    {
        // Ensure test existence.
        if (await managerDbContext.Find<TestDefinition>().MatchID(testId).ExecuteAnyAsync(cancellationToken) ==
            false)
        {
            throw new TestPlatformException("Not Found Test");
        }

        var questionCategoryEntity = mapper.Map<QuestionCategory>(questionCategory);
        questionCategoryEntity.TestId = testId;

        await managerDbContext.SaveAsync(questionCategoryEntity, cancellationToken);

        return mapper.Map<QuestionCategoryViewModel>(questionCategoryEntity);
    }

    public async Task<IEnumerable<QuestionCategoryViewModel>> GetCategories(string testId,
        CancellationToken cancellationToken)
    {
        var entities = await managerDbContext.Find<QuestionCategory>()
            .ManyAsync(c => c.TestId == testId, cancellationToken);
        entities.Add(QuestionCategory.Generic());

        return mapper.Map<IEnumerable<QuestionCategoryViewModel>>(entities).OrderBy(c => c.DisplayOrder).ToList();
    }

    public Task DeleteCategories(string[] ids)
    {
        return managerDbContext.DeleteAsync<QuestionCategory>(ids);
    }
}