using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public interface IQuestionPointCalculationService
    {
         void CalculateTotalPoints(IQuestionViewModel questionViewModel);
    }
}