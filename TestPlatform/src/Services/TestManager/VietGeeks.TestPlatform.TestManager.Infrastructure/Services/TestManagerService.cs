using AutoMapper;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class TestManagerService : ITestManagerService
{
    private readonly IMapper _mapper;
    private readonly TestManagerDbContext _managerDbContext;

    public TestManagerService(IMapper mapper, TestManagerDbContext managerDbContext)
    {
        _mapper = mapper;
        _managerDbContext = managerDbContext;
    }

    public async Task<TestDefinitionViewModel> CreateTestDefinition(NewTestDefinitionViewModel newTest)
    {
        var testEntity = _mapper.Map<TestDefinition>(newTest);
        await _managerDbContext.SaveAsync(testEntity);

        return _mapper.Map<TestDefinitionViewModel>(testEntity);
    }

    public async Task<TestDefinitionViewModel> GetTestDefinition(string id)
    {
        var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();

        return _mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<List<TestDefinitionViewModel>> GetTestDefinitions()
    {
        var entities = await _managerDbContext.Find<TestDefinition>().ExecuteAsync();

        return _mapper.Map<List<TestDefinitionViewModel>>(entities);
    }

    public async Task<TestDefinitionViewModel> UpdateTestDefinition(string id, UpdateTestDefinitionViewModel viewModel)
    {
        var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();
        if (entity == null)
            throw new EntityNotFoundException(id, nameof(TestDefinition));

        var updatedProperties = new List<string>();

        if(viewModel.BasicSettings != null)
        {
            entity.BasicSettings = _mapper.Map<TestBasicSettingsPart>(viewModel.BasicSettings);
            updatedProperties.Add(nameof(TestDefinition.BasicSettings));
        }

        if(viewModel.TestSetSettings != null)
        {
            entity.TestSetSettings = _mapper.Map<TestSetSettingsPart>(viewModel.TestSetSettings);
            updatedProperties.Add(nameof(TestDefinition.TestSetSettings));
        }

        if (updatedProperties.Count > 0)
        {
            var updateResult = await _managerDbContext.SaveOnlyAsync(entity, updatedProperties);
        }
        //todo: check update result to ensure

       return _mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<List<TestCategoryViewModel>> GetTestCategories()
    {
        var entities = await _managerDbContext.Find<TestCategory>().ExecuteAsync();

        return _mapper.Map<List<TestCategoryViewModel>>(entities);
    }

    public async Task<TestCategoryViewModel> CreateTestCategory(NewTestCategoryViewModel newTestCategory)
    {
        var testCategoryEntity = _mapper.Map<TestCategory>(newTestCategory);
        await _managerDbContext.SaveAsync(testCategoryEntity);

        return _mapper.Map<TestCategoryViewModel>(testCategoryEntity);
    }
}