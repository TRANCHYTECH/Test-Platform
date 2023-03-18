using System;
using System.Linq;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;
public static class PointCalculator
{
    public static int CalculatePoint(this QuestionDefinition question, string[]? answerIds)
    {
        if (question.AnswerType == 1)
        {
            var isCorrect = answerIds != null && question.Answers.Any(c => c.Id == answerIds[0] && c.IsCorrect);
            return ((SingleChoiceScoreSettings)question.ScoreSettings).GetPoint(isCorrect);
        }

        if (question.AnswerType == 2)
        {
            var correctedAnswers = question.Answers.Where(c => c.IsCorrect);
            var userCorrected = answerIds == null ? Array.Empty<Answer>() : correctedAnswers.Where(c => answerIds.Contains(c.Id));
            var isCorrect = userCorrected.Count() == correctedAnswers.Count();
            return ((MultipleChoiceScoreSettings)question.ScoreSettings).GetPoint(isCorrect, userCorrected.Sum(c => c.AnswerPoint));
        }

        throw new System.Exception("Not supported");
    }

    public static int GetPoint(this SingleChoiceScoreSettings scoreSettings, bool isCorrect) => isCorrect ? scoreSettings.TotalPoints : scoreSettings.IncorrectPoint.GetValueOrDefault() * -1;

    public static int GetPoint(this MultipleChoiceScoreSettings scoreSettings, bool isCorrect, int correctedAnswerPoints)
    {
        if (isCorrect)
        {
            return scoreSettings.TotalPoints;
        }

        return scoreSettings.IsPartialAnswersEnabled ? correctedAnswerPoints : 0;
    }
}