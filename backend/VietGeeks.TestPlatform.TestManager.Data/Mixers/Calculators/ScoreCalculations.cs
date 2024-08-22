using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Data.Mixers.Calculators;

public static class ScoreCalculations
{
    public static int CalculateScores(this QuestionDefinition question, string[]? answerIds)
    {
        var calculator = ScoreCalculatorFactory.GetCalculator(question.AnswerType);

        return calculator.Calculate(question, answerIds);
    }

    public static bool IsCorrectAnswer(this QuestionDefinition question, string[]? answerIds)
    {
        var calculator = ScoreCalculatorFactory.GetCalculator(question.AnswerType);

        return calculator.IsCorrectAnswer(question, answerIds);
    }
}