using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;
public interface ITestManagerService
{
    Task<TestViewModel> CreateTest(NewTestViewModel newTest);
}