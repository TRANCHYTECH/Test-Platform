using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public static class ScoreCalculations
{
    public static int CalculateScores(this QuestionDefinition question, string[]? answerIds)
    {
        var calculator = ScoreCalculatorFactory.GetCalculator((AnswerType)question.AnswerType);

        return calculator.Calculate(question, answerIds);
    }

    public static bool IsCorrectAnswer(this QuestionDefinition question, string[]? answerIds)
    {
        var calculator = ScoreCalculatorFactory.GetCalculator((AnswerType)question.AnswerType);

        return calculator.IsCorrectAnswer(question, answerIds);
    }
}