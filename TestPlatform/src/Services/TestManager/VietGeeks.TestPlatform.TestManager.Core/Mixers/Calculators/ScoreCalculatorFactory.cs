using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

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
                throw new System.Exception($"Answer type {answerType} is not supported");
        }
    }
}
