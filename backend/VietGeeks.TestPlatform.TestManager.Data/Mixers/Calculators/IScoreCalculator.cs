using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Data.Mixers.Calculators;

public interface IScoreCalculator
{
    bool IsCorrectAnswer(QuestionDefinition question, string[]? answerIds);
    int Calculate(QuestionDefinition question, string[]? answerIds);
}