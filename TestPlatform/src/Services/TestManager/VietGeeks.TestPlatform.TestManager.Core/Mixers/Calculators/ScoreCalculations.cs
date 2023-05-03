using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public static class ScoreCalculations
{
    public static int CalculateScores(this QuestionDefinition question, string[]? answerIds)
    {
        var calculator = ScoreCalculatorFactory.GetCalculator(question);

        return calculator.Calculate(question, answerIds);
    }
}