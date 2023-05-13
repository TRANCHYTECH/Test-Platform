using System.Linq;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public class SingleChoiceScoreCalculator : IScoreCalculator
{
    public int Calculate(QuestionDefinition question, string[]? answerIds)
    {
        if (question.ScoreSettings is not SingleChoiceScoreSettings scoreSettings) {
            throw new System.Exception("ScoreSettings is not SingleChoiceScoreSettings");
        }

        var isCorrect = IsCorrectAnswer(question, answerIds);
        return isCorrect ? scoreSettings.CorrectPoint : scoreSettings.IncorrectPoint;
    }

    public bool IsCorrectAnswer(QuestionDefinition question, string[]? answerIds)
    {
        return answerIds != null && question.Answers.Any(c => c.Id == answerIds[0] && c.IsCorrect);
    }
}
