using System.Globalization;
using AutoMapper;
using FluentValidation;
using LexoAlgorithm;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public class QuestionManagerService(
        IMapper mapper,
        IClock clock,
        IValidator<QuestionDefinition> questionDefinitionValidator,
        TestManagerDbContext managerDbContext,
        IQuestionPointCalculationService questionPointCalculationService)
        : IQuestionManagerService
    {
        public async Task<IEnumerable<QuestionViewModel>> GetQuestions(string testId,
            CancellationToken cancellationToken)
        {
            var entities = await managerDbContext.Find<QuestionDefinition>()
                .ManyAsync(q => q.TestId == testId, cancellationToken);

            entities = AssignQuestionNumbers(entities);

            return mapper.Map<List<QuestionViewModel>>(entities);
        }

        public async Task<PagedSearchResult<QuestionViewModel>> GetQuestions(string testId, int pageNumber,
            int pageSize, CancellationToken cancellationToken)
        {
            var entities = await managerDbContext.PagedSearch<QuestionDefinition>()
                .Match(q => q.TestId == testId)
                .Sort(q => q.Order, Order.Ascending)
                .PageSize(pageSize)
                .PageNumber(pageNumber)
                .ExecuteAsync(cancellationToken);

            var processedEntities = AssignQuestionNumbers(entities.Results.ToList(), pageNumber, pageSize);

            return new PagedSearchResult<QuestionViewModel>
            {
                Results = mapper.Map<List<QuestionViewModel>>(processedEntities),
                TotalCount = entities.TotalCount,
                PageCount = entities.PageCount
            };
        }

        public async Task<QuestionViewModel> GetQuestion(string id, CancellationToken cancellationToken)
        {
            var entity = await managerDbContext.Find<QuestionDefinition>().MatchID(id)
                .ExecuteFirstAsync(cancellationToken);

            // Get question number.
            entity.QuestionNo = await GetQuestionNo(entity);

            return mapper.Map<QuestionViewModel>(entity);
        }

        public async Task<QuestionViewModel> CreateQuestion(string testId,
            CreateOrUpdateQuestionViewModel questionViewModel, CancellationToken cancellationToken)
        {
            var entity = mapper.Map<QuestionDefinition>(questionViewModel);
            entity.TestId = testId;
            entity.Order = GenerateOrderSequence();
            entity.ScoreSettings.TotalPoints = questionPointCalculationService.CalculateTotalPoints(entity);

            await questionDefinitionValidator.TryValidate(entity);

            await managerDbContext.SaveAsync(entity, cancellationToken);

            return mapper.Map<QuestionViewModel>(entity);
        }

        public async Task<QuestionViewModel> UpdateQuestion(string id,
            CreateOrUpdateQuestionViewModel questionViewModel, CancellationToken cancellationToken)
        {
            var entity =
                await managerDbContext.Find<QuestionDefinition>().MatchID(id).ExecuteFirstAsync(cancellationToken) ??
                throw new EntityNotFoundException(id, nameof(QuestionDefinition));
            entity = mapper.Map(questionViewModel, entity);
            entity.ScoreSettings.TotalPoints = questionPointCalculationService.CalculateTotalPoints(entity);

            await questionDefinitionValidator.TryValidate(entity);

            await managerDbContext.SaveAsync(entity, cancellationToken);

            return mapper.Map<QuestionViewModel>(entity);
        }

        public async Task<IEnumerable<QuestionSummaryViewModel>> GetQuestionSummary(string testId,
            CancellationToken cancellationToken)
        {
            var entities = await managerDbContext.Find<QuestionDefinition>()
                .Project(q => new QuestionDefinition
                    { ID = q.ID, CategoryId = q.CategoryId, ScoreSettings = q.ScoreSettings })
                .ManyAsync(q => q.TestId == testId, cancellationToken);

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
            var result = await managerDbContext.DeleteAsync<QuestionDefinition>(id);
            if (result.DeletedCount == 0)
            {
                throw new TestPlatformException("Not found question");
            }
        }

        public async Task UpdateQuestionOrders(string testId, UpdateQuestionOrderViewModel[] viewModel,
            CancellationToken cancellation)
        {
            //todo: verify condition if test is ended or activated, so not allow to update.
            var bulkUpdate = managerDbContext.Update<QuestionDefinition>();
            foreach (var questionOrder in viewModel)
            {
                bulkUpdate.MatchID(questionOrder.Id)
                    .Modify(c => c.Order, questionOrder.Order)
                    .AddToQueue();
            }

            var updateResult = await bulkUpdate.ExecuteAsync();
            if (!updateResult.IsAcknowledged || updateResult.ModifiedCount != viewModel.Count())
            {
                //todo: refine this exception.
                throw new Exception("Could not update");
            }
        }

        private string GenerateOrderSequence()
        {
            var @decimal = clock.UtcNow.Subtract(new DateTime(2022, 1, 30)).TotalMilliseconds
                .ToString(NumberFormatInfo.InvariantInfo);
            @decimal = @decimal.Replace(".", "a").Insert(6, ":");
            var rank = LexoRank.Parse($"0|{@decimal}");

            return rank.Format();
        }

        private List<QuestionDefinition> AssignQuestionNumbers(List<QuestionDefinition> questions, int pageNumber,
            int pageSize)
        {
            var startedNumber = (pageNumber - 1) * pageSize + 1;
            for (var i = 0; i < questions.Count; i++)
            {
                questions[i].QuestionNo = startedNumber + i;
            }

            return questions;
        }

        private List<QuestionDefinition> AssignQuestionNumbers(List<QuestionDefinition> questions)
        {
            var sortedQuestions = questions.OrderBy(c => c.Order).ToList();
            for (var i = 0; i < questions.Count; i++)
            {
                sortedQuestions[i].QuestionNo = i + 1;
            }

            return sortedQuestions;
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

            var result = await managerDbContext
                .Find<QuestionDefinition>()
                .Match(pipeline)
                .Project(c => new QuestionDefinition { Order = c.Order })
                .ExecuteAsync();

            if (result.Count == 0)
            {
                throw new TestPlatformException("Invalid parameter");
            }

            return result.Count;
        }
    }
}