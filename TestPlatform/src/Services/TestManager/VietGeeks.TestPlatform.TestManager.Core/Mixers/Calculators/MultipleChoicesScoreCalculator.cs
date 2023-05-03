using System;
using System.Linq;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public class MultipleChoicesScoreCalculator : IScoreCalculator
{
    public int Calculate(QuestionDefinition question, string[]? answerIds)
    {
        if (question.ScoreSettings is not MultipleChoiceScoreSettings scoreSettings) {
            throw new Exception("scoreSettings is not type of MultipleChoiceScoreSettings");
        }

        var correctedAnswers = question.Answers.Where(c => c.IsCorrect);
        var userCorrected = answerIds == null ? Array.Empty<Answer>() : correctedAnswers.Where(c => answerIds.Contains(c.Id));

        var isFullyCorrect = userCorrected.Count() == correctedAnswers.Count();
        var totalPoints = 0;

        if (scoreSettings.IsPartialAnswersEnabled)
        {
            totalPoints = userCorrected.Sum(c => c.AnswerPoint);

            if (isFullyCorrect)
            {
                totalPoints += scoreSettings.BonusPoints.GetValueOrDefault();
            }

            return totalPoints;
        }
        else if (isFullyCorrect)
        {
            totalPoints = scoreSettings.TotalPoints;
        }

        return totalPoints;
    }
}
