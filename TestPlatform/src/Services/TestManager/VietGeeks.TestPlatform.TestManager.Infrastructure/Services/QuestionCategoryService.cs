﻿using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;

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

        public async Task<QuestionCategoryViewModel> CreateQuestionCategory(NewQuestionCategoryViewModel questionCategory, CancellationToken cancellationToken)
        {
            var questionCategoryEntity = _mapper.Map<QuestionCategory>(questionCategory);
            await _managerDbContext.SaveAsync(questionCategoryEntity, cancellationToken);

            return _mapper.Map<QuestionCategoryViewModel>(questionCategoryEntity);
        }

        public async Task<IEnumerable<QuestionCategoryViewModel>> GetCategories(CancellationToken cancellationToken)
        {
            var entities = await _managerDbContext.Find<QuestionCategory>().ManyAsync(c => c is QuestionCategory);
            entities.Add(QuestionCategory.Generic());

            return _mapper.Map<IEnumerable<QuestionCategoryViewModel>>(entities).OrderBy(c => c.DisplayOrder).ToList();;
        }
    }
}
