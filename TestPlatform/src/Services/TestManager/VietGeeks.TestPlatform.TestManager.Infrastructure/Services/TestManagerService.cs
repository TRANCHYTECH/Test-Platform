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

        public async Task<TestViewModel> CreateTest(NewTestViewModel newTest)
        {
            var testEntity = _mapper.Map<MyTest>(newTest);
            await _managerDbContext.SaveAsync(testEntity);

            return _mapper.Map<TestViewModel>(testEntity);
        }
    }
}