using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;
public interface ITestManagerService
{
    Task<List<TestDefinitionOverview>> GetTestDefinitionOverviews();
    Task<TestDefinitionViewModel> CreateTestDefinition(NewTestDefinitionViewModel newTest);
    Task<TestDefinitionViewModel> GetTestDefinition(string id);
    Task<TestDefinitionViewModel> ActivateTestDefinition(string id);
    Task<TestDefinitionViewModel> UpdateTestDefinition(string id, UpdateTestDefinitionViewModel viewModel);
    Task<TestCategoryViewModel> CreateTestCategory(NewTestCategoryViewModel newTestCategory);
    Task<List<TestCategoryViewModel>> GetTestCategories();
    Task<TestDefinitionViewModel> EndTestDefinition(string id);
    Task<TestDefinitionViewModel> RestartTestDefinition(string id);
}