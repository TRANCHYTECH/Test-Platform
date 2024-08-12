using AutoMapper;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public class QuestionCategoryService : IQuestionCategoryService
    {
        private readonly IMapper _mapper;
        private readonly TestManagerDbContext _managerDbContext;

        public QuestionCategoryService(IMapper mapper, TestManagerDbContext managerDbContext)
        {
            _mapper = mapper;
            _managerDbContext = managerDbContext;
        }

        public async Task<QuestionCategoryViewModel> CreateCategory(string testId, NewQuestionCategoryViewModel questionCategory, CancellationToken cancellationToken)
        {
            // Ensure test existence.
            if (await _managerDbContext.Find<TestDefinition>().MatchID(testId).ExecuteAnyAsync() == false)
                throw new TestPlatformException("Not Found Test");

            var questionCategoryEntity = _mapper.Map<QuestionCategory>(questionCategory);
            questionCategoryEntity.TestId = testId;

            await _managerDbContext.SaveAsync(questionCategoryEntity, cancellationToken);

            return _mapper.Map<QuestionCategoryViewModel>(questionCategoryEntity);
        }

        public async Task<IEnumerable<QuestionCategoryViewModel>> GetCategories(string testId, CancellationToken cancellationToken)
        {
            var entities = await _managerDbContext.Find<QuestionCategory>().ManyAsync(c => c is QuestionCategory && c.TestId == testId);
            entities.Add(QuestionCategory.Generic());

            return _mapper.Map<IEnumerable<QuestionCategoryViewModel>>(entities).OrderBy(c => c.DisplayOrder).ToList(); ;
        }

        public Task DeleteCategories(string[] ids)
        {
            return _managerDbContext.DeleteAsync<QuestionCategory>(ids);
        }
    }
}
