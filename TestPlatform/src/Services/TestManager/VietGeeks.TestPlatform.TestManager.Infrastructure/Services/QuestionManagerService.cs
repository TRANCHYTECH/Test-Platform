using AutoMapper;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class QuestionManagerService : IQuestionManagerService
{
    private readonly IMapper _mapper;
    private readonly TestManagerDbContext _managerDbContext;

    public QuestionManagerService(IMapper mapper, TestManagerDbContext managerDbContext)
    {
        _mapper = mapper;
        _managerDbContext = managerDbContext;
    }

    public async Task<IEnumerable<QuestionViewModel>> GetQuestions(string testId, CancellationToken cancellationToken)
    {
        var entities = await _managerDbContext.Find<QuestionDefinition>().ManyAsync(q => q.TestId == testId, cancellationToken);

        return _mapper.Map<List<QuestionViewModel>>(entities);
    }

    public async Task<QuestionViewModel> GetQuestion(string id, CancellationToken cancellationToken)
    {
        var entity = await _managerDbContext.Find<QuestionDefinition>().MatchID(id).ExecuteFirstAsync();

        return _mapper.Map<QuestionViewModel>(entity);
    }

    public async Task<QuestionViewModel> CreateQuestion(string testId, NewQuestionViewModel questionViewModel, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<QuestionDefinition>(questionViewModel);
        entity.TestId = testId;
        await _managerDbContext.SaveAsync(entity, cancellationToken);

        return _mapper.Map<QuestionViewModel>(entity);
    }

    public async Task<QuestionViewModel> UpdateQuestion(string id, QuestionViewModel questionViewModel, CancellationToken cancellationToken)
    {
        var existingEntity = await _managerDbContext.Find<QuestionDefinition>().MatchID(id).ExecuteFirstAsync(cancellationToken);
        if (existingEntity == null)
            throw new EntityNotFoundException(id, nameof(QuestionDefinition));

        var updatedEntity = _mapper.Map<QuestionDefinition>(questionViewModel);
        updatedEntity.ID = id;
        updatedEntity.TestId = existingEntity.TestId;

        await _managerDbContext.SaveAsync(updatedEntity, cancellationToken);

        return _mapper.Map<QuestionViewModel>(updatedEntity);
    }

    public async Task<IEnumerable<QuestionSummaryViewModel>> GetQuestionSummary(string testId, CancellationToken cancellationToken)
    {
        var entities = await _managerDbContext.Find<QuestionDefinition>().Project(q => new QuestionDefinition { ID = q.ID, CategoryId = q.CategoryId }).ManyAsync(q => q.TestId == testId, cancellationToken);

        return entities.GroupBy(c => c.CategoryId).Select(c => new QuestionSummaryViewModel
        {
            CategoryId = c.Key,
            NumberOfQuestions = c.Count()
        });
    }
}