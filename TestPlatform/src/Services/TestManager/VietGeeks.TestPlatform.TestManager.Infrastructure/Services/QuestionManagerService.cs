using AutoMapper;
using FluentValidation;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class QuestionManagerService : IQuestionManagerService
{
    private readonly IMapper _mapper;
    private readonly IClock _clock;
    private readonly IValidator<QuestionDefinition> _questionDefinitionValidator;
    private readonly TestManagerDbContext _managerDbContext;
    private readonly IQuestionPointCalculationService _questionPointCalculationService;

    public QuestionManagerService(
        IMapper mapper,
        IClock clock,
        IValidator<QuestionDefinition> questionDefinitionValidator,
        TestManagerDbContext managerDbContext,
        IQuestionPointCalculationService questionPointCalculationService)
    {
        _mapper = mapper;
        _clock = clock;
        _questionDefinitionValidator = questionDefinitionValidator;
        _managerDbContext = managerDbContext;
        _questionPointCalculationService = questionPointCalculationService;
    }

    public async Task<IEnumerable<QuestionViewModel>> GetQuestions(string testId, CancellationToken cancellationToken)
    {
        var entities = await _managerDbContext.Find<QuestionDefinition>().ManyAsync(q => q.TestId == testId, cancellationToken);

        AssignQuestionNumbers(entities);

        return _mapper.Map<List<QuestionViewModel>>(entities);
    }

    public async Task<IEnumerable<QuestionViewModel>> GetQuestions(string testId, int pageIndex, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _managerDbContext.Find<QuestionDefinition>()
            .Skip(pageIndex * pageSize)
            .Limit(pageSize)
            .Sort(q => q.Order, Order.Ascending)
            .ManyAsync(q => q.TestId == testId, cancellationToken);

        return _mapper.Map<List<QuestionViewModel>>(entities);
    }

    public async Task<QuestionViewModel> GetQuestion(string id, CancellationToken cancellationToken)
    {
        var entity = await _managerDbContext.Find<QuestionDefinition>().MatchID(id).ExecuteFirstAsync(cancellationToken);

        // Get question number.
        entity.QuestionNo = await GetQuestionNo(entity);

        return _mapper.Map<QuestionViewModel>(entity);
    }

    public async Task<QuestionViewModel> CreateQuestion(string testId, CreateOrUpdateQuestionViewModel questionViewModel, CancellationToken cancellationToken)
    {
        var entity = _mapper.Map<QuestionDefinition>(questionViewModel);
        entity.TestId = testId;
        entity.Order = _clock.UtcNow.ToString("yyyyMMddHHmmssfff");
        entity.ScoreSettings.TotalPoints = _questionPointCalculationService.CalculateTotalPoints(entity);

        await _questionDefinitionValidator.TryValidate(entity);

        await _managerDbContext.SaveAsync(entity, cancellationToken);

        return _mapper.Map<QuestionViewModel>(entity);
    }

    public async Task<QuestionViewModel> UpdateQuestion(string id, CreateOrUpdateQuestionViewModel questionViewModel, CancellationToken cancellationToken)
    {
        var entity = await _managerDbContext.Find<QuestionDefinition>().MatchID(id).ExecuteFirstAsync(cancellationToken) ?? throw new EntityNotFoundException(id, nameof(QuestionDefinition));
        entity = _mapper.Map(questionViewModel, entity);
        entity.ScoreSettings.TotalPoints = _questionPointCalculationService.CalculateTotalPoints(entity);

        await _questionDefinitionValidator.TryValidate(entity);

        await _managerDbContext.SaveAsync(entity, cancellationToken);

        return _mapper.Map<QuestionViewModel>(entity);
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

    private void AssignQuestionNumbers(List<QuestionDefinition> questions)
    {
        questions = questions.OrderBy(c => c.Order).ToList();
        for (int i = 0; i < questions.Count; i++)
        {
            questions[i].QuestionNo = i + 1;
        }
    }

    //todo: Improve performance by using cache tech?
    private async Task<int> GetQuestionNo(QuestionDefinition question)
    {
        var pipeline = new Template<QuestionDefinition>(@"
        {
            TestId: '<TestId>',
            $expr: { $lte: [{ $strcasecmp: ['$Order', '<Order>'] }, 0] }
        }
        ")
        .Tag("TestId", question.TestId)
        .Tag("Order", question.Order);

        var result = await _managerDbContext
        .Find<QuestionDefinition>()
        .Match(pipeline)
        .Project(c => new QuestionDefinition { Order = c.Order })
        .ExecuteAsync();

        if (result.Count == 0)
            throw new TestPlatformException("Invalid parameter");

        return result.Count;
    }
}