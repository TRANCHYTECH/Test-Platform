using System.Reflection;
using AutoMapper;
using FluentValidation;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefintion;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class QuestionManagerService : IQuestionManagerService
{
    private readonly IMapper _mapper;
    private readonly IValidator<QuestionDefinition> _questionDefinitionValidator;
    private readonly TestManagerDbContext _managerDbContext;
    private readonly IQuestionPointCalculationService _questionPointCalculationService;

    public QuestionManagerService(
        IMapper mapper,
        IValidator<QuestionDefinition> questionDefinitionValidator,
        TestManagerDbContext managerDbContext,
        IQuestionPointCalculationService questionPointCalculationService)
    {
        _mapper = mapper;
        _questionDefinitionValidator = questionDefinitionValidator;
        _managerDbContext = managerDbContext;
        _questionPointCalculationService = questionPointCalculationService;
    }

    public async Task<IEnumerable<QuestionViewModel>> GetQuestions(string testId, CancellationToken cancellationToken)
    {
        var entities = await _managerDbContext.Find<QuestionDefinition>().ManyAsync(q => q.TestId == testId, cancellationToken);

        return _mapper.Map<List<QuestionViewModel>>(entities);
    }

    public async Task<IEnumerable<QuestionViewModel>> GetQuestions(string testId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _managerDbContext.Find<QuestionDefinition>()
            .Skip(pageIndex * pageSize)
            .Limit(pageSize)
            .Sort(q => q.QuestionNo, Order.Ascending)
            .ManyAsync(q => q.TestId == testId, cancellationToken);

        return _mapper.Map<List<QuestionViewModel>>(entities);
    }

    public async Task<QuestionViewModel> GetQuestion(string id, CancellationToken cancellationToken)
    {
        var entity = await _managerDbContext.Find<QuestionDefinition>().MatchID(id).ExecuteFirstAsync(cancellationToken);

        return _mapper.Map<QuestionViewModel>(entity);
    }

    public async Task<QuestionViewModel> CreateQuestion(string testId, NewQuestionViewModel questionViewModel, CancellationToken cancellationToken)
    {
        _questionPointCalculationService.CalculateTotalPoints(questionViewModel);
        var entity = _mapper.Map<QuestionDefinition>(questionViewModel);
        entity.TestId = testId;

        await _questionDefinitionValidator.TryValidate(entity);

        await _managerDbContext.SaveAsync(entity, cancellationToken);

        return _mapper.Map<QuestionViewModel>(entity);
    }

    public async Task<QuestionViewModel> UpdateQuestion(string id, QuestionViewModel questionViewModel, CancellationToken cancellationToken)
    {
        var existingEntity = await _managerDbContext.Find<QuestionDefinition>().MatchID(id).ExecuteFirstAsync(cancellationToken) ?? throw new EntityNotFoundException(id, nameof(QuestionDefinition));

        _questionPointCalculationService.CalculateTotalPoints(questionViewModel);
        var updatedEntity = _mapper.Map<QuestionDefinition>(questionViewModel);
        updatedEntity.ID = id;
        updatedEntity.TestId = existingEntity.TestId;

        await _questionDefinitionValidator.TryValidate(updatedEntity);

        await _managerDbContext.SaveAsync(updatedEntity, cancellationToken);

        return _mapper.Map<QuestionViewModel>(updatedEntity);
    }

    public async Task<IEnumerable<QuestionSummaryViewModel>> GetQuestionSummary(string testId, CancellationToken cancellationToken)
    {
        var entities = await _managerDbContext.Find<QuestionDefinition>().Project(q => new QuestionDefinition { ID = q.ID, CategoryId = q.CategoryId, ScoreSettings = q.ScoreSettings }).ManyAsync(q => q.TestId == testId, cancellationToken);

        return entities.GroupBy(c => c.CategoryId).Select(c => new QuestionSummaryViewModel
        {
            CategoryId = c.Key,
            NumberOfQuestions = c.Count(),
            TotalPoints = c.Sum(q => q.ScoreSettings.TotalPoints)
        });
    }

    public async Task<int> GetTotalPoints(string testId, CancellationToken cancellationToken)
    {
        var summary = await GetQuestionSummary(testId, cancellationToken);

        return summary.Sum(c => c.TotalPoints);
    }

    public async Task DeleteQuestion(string id, CancellationToken cancellationToken)
    {
        //todo: verify condition if test is ended or activated, so not allow to modify/delete
        var result = await _managerDbContext.DeleteAsync<QuestionDefinition>(id);
        if (result.DeletedCount == 0)
            throw new TestPlatformException("Not found question");
    }
}