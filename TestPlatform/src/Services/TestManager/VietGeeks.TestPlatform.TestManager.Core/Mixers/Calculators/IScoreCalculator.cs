using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public interface IScoreCalculator
{
    int Calculate(QuestionDefinition question, string[]? answerIds);
}