using HotChocolate.Authorization;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.GraphQL;

[ExtendObjectType(OperationTypeNames.Mutation)]
[Authorize]
public class TestCategoryMutations
{
    public async Task<TestCategoryViewModel> Create(NewTestCategoryViewModel viewModel, [Service] ITestCategoryService testCatalogService)
    {
        var createdTestCategory = await testCatalogService.CreateTestCategory(viewModel);

        return createdTestCategory;
    }
}


