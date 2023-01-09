using AutoMapper;
using MongoDB.Driver;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure
{
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

            var updatedProperties = new List<string>();
            if(viewModel.BasicSettings != null)
            {
                _mapper.Map(viewModel.BasicSettings, entity.BasicSettings);
                updatedProperties.Add(nameof(TestDefinition.BasicSettings));
            }

            if (updatedProperties.Count > 0)
            {
                var updateResult = await _managerDbContext.SaveOnlyAsync(entity, updatedProperties);
            }
            //todo: check update result to ensure

           return _mapper.Map<TestDefinitionViewModel>(entity);
        }
    }
}