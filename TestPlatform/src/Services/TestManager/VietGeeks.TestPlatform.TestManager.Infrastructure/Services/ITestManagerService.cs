using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;
public interface ITestManagerService
{
    Task<TestDefinitionViewModel> CreateTestDefinition(NewTestDefinitionViewModel newTest);
    Task<TestDefinitionViewModel> GetTestDefinition(string id);
    Task<TestDefinitionViewModel> ActivateTestDefinition(string id);
    Task<List<TestDefinitionViewModel>> GetTestDefinitions();
    Task<TestDefinitionViewModel> UpdateTestDefinition(string id, UpdateTestDefinitionViewModel viewModel);

    Task<TestCategoryViewModel> CreateTestCategory(NewTestCategoryViewModel newTestCategory);
    Task<List<TestCategoryViewModel>> GetTestCategories();
    Task<ReadyForActivationStatus> CheckTestDefinitionReadyForActivation(string id);
    Task<TestDefinitionViewModel> EndTestDefinition(string id);
}