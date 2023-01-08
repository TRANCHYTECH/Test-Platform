using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;
public interface ITestManagerService
{
    Task<TestDefinitionViewModel> CreateTest(NewTestDefinitionViewModel newTest);
    Task<TestDefinitionViewModel> GetTestDefinition(string id);
    Task<List<TestDefinitionViewModel>> GetTestDefinitions();
    Task<TestDefinitionViewModel> UpdateTestBasicSettings(string id, UpdateTestDefinitionViewModel viewModel);
}