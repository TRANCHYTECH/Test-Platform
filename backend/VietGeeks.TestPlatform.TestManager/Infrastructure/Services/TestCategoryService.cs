using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public interface ITestCategoryService
{
    Task<TestCategoryViewModel> CreateTestCategory(NewTestCategoryViewModel newTestCategory);
    Task<List<TestCategoryViewModel>> GetTestCategories();
    Task<bool> CheckTestCategoryExistence(string id);
    Task DeleteTestCategories(string[] ids);
}

public class TestCategoryService(TestManagerDbContext dbContext, IMapper mapper) : ITestCategoryService
{
    public async Task<List<TestCategoryViewModel>> GetTestCategories()
    {
        var entities = await dbContext.Find<TestCategory>().ManyAsync(c => c is TestCategory);
        entities.Add(TestCategory.Uncategorized());

        return mapper.Map<List<TestCategoryViewModel>>(entities).OrderBy(c => c.DisplayOrder).ToList();
    }

    public async Task<TestCategoryViewModel> CreateTestCategory(NewTestCategoryViewModel newTestCategory)
    {
        var testCategoryEntity = mapper.Map<TestCategory>(newTestCategory);
        await dbContext.SaveAsync(testCategoryEntity);

        return mapper.Map<TestCategoryViewModel>(testCategoryEntity);
    }

    public async Task<bool> CheckTestCategoryExistence(string id)
    {
        return id == TestCategory.UncategorizedId || await dbContext.Find<TestCategory>().MatchID(id).ExecuteAnyAsync();
    }

    public async Task DeleteTestCategories(string[] ids)
    {
        await dbContext.DeleteAsync<TestCategory>(ids);
    }
}

