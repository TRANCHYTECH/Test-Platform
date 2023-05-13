using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public interface IScoreCalculator
{
    bool IsCorrectAnswer(QuestionDefinition question, string[]? answerIds);
    int Calculate(QuestionDefinition question, string[]? answerIds);
}