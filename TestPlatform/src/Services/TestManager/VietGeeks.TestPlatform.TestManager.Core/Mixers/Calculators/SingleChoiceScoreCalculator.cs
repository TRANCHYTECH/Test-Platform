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

        var isCorrect = answerIds != null && question.Answers.Any(c => c.Id == answerIds[0] && c.IsCorrect);
        return isCorrect ? scoreSettings.TotalPoints : scoreSettings.IncorrectPoint.GetValueOrDefault() * -1;
    }
}
