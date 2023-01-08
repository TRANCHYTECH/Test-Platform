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

        public async Task<TestDefinitionViewModel> CreateTest(NewTestDefinitionViewModel newTest)
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

        public async Task<TestDefinitionViewModel> UpdateTestBasicSettings(string id, UpdateTestDefinitionViewModel viewModel)
        {
            var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();
            if(viewModel.BasicSettings != null)
            {
                _mapper.Map(viewModel.BasicSettings, entity.BasicSettings);
            }

            var updateResult = await _managerDbContext.SaveOnlyAsync(entity, e => new { e.BasicSettings });
            //todo: check update result to ensure

           return _mapper.Map<TestDefinitionViewModel>(entity);
        }
    }
}