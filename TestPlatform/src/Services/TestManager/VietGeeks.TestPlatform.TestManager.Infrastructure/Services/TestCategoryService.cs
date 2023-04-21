using System;
using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public interface ITestCategoryService
{
    Task<TestCategoryViewModel> CreateTestCategory(NewTestCategoryViewModel newTestCategory);
    Task<List<TestCategoryViewModel>> GetTestCategories();
    Task<bool> CheckTestCategoryExistence(string id);
}

public class TestCategoryService : ITestCategoryService
{
    private readonly TestManagerDbContext _dbContext;
    private readonly IMapper _mapper;

    public TestCategoryService(TestManagerDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public async Task<List<TestCategoryViewModel>> GetTestCategories()
    {
        var entities = await _dbContext.Find<TestCategory>().ManyAsync(c => c is TestCategory);
        entities.Add(TestCategory.Uncategorized());

        return _mapper.Map<List<TestCategoryViewModel>>(entities).OrderBy(c => c.DisplayOrder).ToList();
    }

    public async Task<TestCategoryViewModel> CreateTestCategory(NewTestCategoryViewModel newTestCategory)
    {
        var testCategoryEntity = _mapper.Map<TestCategory>(newTestCategory);
        await _dbContext.SaveAsync(testCategoryEntity);

        return _mapper.Map<TestCategoryViewModel>(testCategoryEntity);
    }

    public async Task<bool> CheckTestCategoryExistence(string id)
    {
        return id == TestCategory.UncategorizedId || await _dbContext.Find<TestCategory>().MatchID(id).ExecuteAnyAsync();
    }
}

