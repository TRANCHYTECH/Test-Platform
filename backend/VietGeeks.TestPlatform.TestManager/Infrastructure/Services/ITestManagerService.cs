using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;
public interface ITestManagerService
{
    Task<PagedSearchResult<TestDefinitionOverview>> GetTestDefinitionOverviews(int pageNumber, int pageSize, CancellationToken cancellationToken);
    Task<TestDefinitionViewModel> CreateTestDefinition(NewTestDefinitionViewModel newTest);
    Task<TestDefinitionViewModel> GetTestDefinition(string id);
    Task<TestDefinitionViewModel> ActivateTestDefinition(string id);
    Task<TestDefinitionViewModel> UpdateTestDefinition(string id, UpdateTestDefinitionViewModel viewModel);
    Task<TestDefinitionViewModel> EndTestDefinition(string id);
    Task<TestDefinitionViewModel> RestartTestDefinition(string id);
    Task<List<dynamic>> GetTestInvitationEvents(TestInvitationStatsInput input);
    Task<dynamic> GenerateAccessCodes(string id, int quantity);
    Task<dynamic> RemoveAccessCodes(string id, string[] codes);
    Task SendAccessCodes(string id, string[] codes);  
}