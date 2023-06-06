using HotChocolate.Authorization;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.GraphQL;

[ExtendObjectType(OperationTypeNames.Query)]
public class TestCategoryQueries
{
    [Authorize]
    public Task<List<TestCategoryViewModel>> TestCategories([Service] ITestCategoryService testCategoryService)
    {
        return testCategoryService.GetTestCategories();
    }
}
