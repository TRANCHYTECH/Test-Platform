using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public interface IQuestionPointCalculationService
    {
         int CalculateTotalPoints(QuestionDefinition question);
    }
}