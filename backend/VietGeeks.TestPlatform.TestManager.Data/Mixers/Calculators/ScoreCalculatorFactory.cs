using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Data.Mixers.Calculators;

public static class ScoreCalculatorFactory
{
    public static IScoreCalculator GetCalculator(AnswerType answerType)
    {
        switch (answerType)
        {
            case AnswerType.SingleChoice:
                return new SingleChoiceScoreCalculator();
            case AnswerType.MultipleChoice:
                return new MultipleChoicesScoreCalculator();
            default:
                throw new ArgumentOutOfRangeException($"Answer type {answerType} is not supported");
        }
    }
}
