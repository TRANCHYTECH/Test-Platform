using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public static class ScoreCalculatorFactory
{
    public static IScoreCalculator GetCalculator(this QuestionDefinition questionDefinition)
    {
        switch ((AnswerType)questionDefinition.AnswerType)
        {
            case AnswerType.SingleChoice:
                return new SingleChoiceScoreCalculator();
            case AnswerType.MultipleChoice:
                return new MultipleChoicesScoreCalculator();
            default:
                throw new System.Exception($"Answer type {questionDefinition.AnswerType} is not supported");
        }
    }
}
