﻿using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;
public interface ITestManagerService
{
    Task<TestDefinitionViewModel> CreateTestDefinition(NewTestDefinitionViewModel newTest);
    Task<TestDefinitionViewModel> GetTestDefinition(string id);
    Task<List<TestDefinitionViewModel>> GetTestDefinitions();
    Task<TestDefinitionViewModel> UpdateTestDefinition(string id, UpdateTestDefinitionViewModel viewModel);
}