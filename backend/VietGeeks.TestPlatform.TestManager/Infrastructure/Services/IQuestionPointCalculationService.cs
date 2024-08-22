using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public interface IQuestionPointCalculationService
    {
        int CalculateTotalPoints(QuestionDefinition question);
    }
}