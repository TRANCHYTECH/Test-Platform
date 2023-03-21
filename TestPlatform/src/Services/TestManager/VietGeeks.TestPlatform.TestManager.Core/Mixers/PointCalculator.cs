using System;
using System.Collections.Generic;
using System.Linq;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Core.ReadonlyModels;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public static class PointCalculator
{
    public static List<AggregatedGrading> CalculateGrading(this GradingSettingsPart settings, decimal finalMark, decimal totalPoints)
    {
        var result = new List<AggregatedGrading>();
        foreach (var setting in settings.GradingCriterias)
        {
            if (setting.Value is PassMaskCriteria passMaskCriteria)
            {
                var v = finalMark;
                if (passMaskCriteria.Unit == RangeUnit.Percent)
                {
                    v = (finalMark / totalPoints) * 100;
                }

                result.Add(new() { GradingType = GradingCriteriaConfigType.PassMask,  PassMark = v >= passMaskCriteria.Value });
            }
            else if (setting.Value is GradeRangeCriteria gradeRangeCriteria)
            {
                var v = finalMark;
                if (gradeRangeCriteria.Unit == RangeUnit.Percent)
                {
                    v = (finalMark / totalPoints) * 100;
                }

                var matchedLevel = gradeRangeCriteria.Details.OrderBy(c => c.To).First(c => c.To >= v);

                result.Add(new() { GradingType = GradingCriteriaConfigType.GradeRanges, Grades = matchedLevel.Grades });
            }
        }

        return result;
    }

    public static int CalculateMark(this QuestionDefinition question, string[]? answerIds)
    {
        // Single choice questions.
        if (question.AnswerType == 1)
        {
            var isCorrect = answerIds != null && question.Answers.Any(c => c.Id == answerIds[0] && c.IsCorrect);
            return ((SingleChoiceScoreSettings)question.ScoreSettings).GetPoint(isCorrect);
        }

        // Multiple choices question.
        if (question.AnswerType == 2)
        {
            var correctedAnswers = question.Answers.Where(c => c.IsCorrect);
            var userCorrected = answerIds == null ? Array.Empty<Answer>() : correctedAnswers.Where(c => answerIds.Contains(c.Id));
            return ((MultipleChoiceScoreSettings)question.ScoreSettings).GetPoint(correctedAnswers, userCorrected);
        }

        throw new Exception("NotSupportedAnswerType");
    }

    public static int GetPoint(this SingleChoiceScoreSettings scoreSettings, bool isCorrect) => isCorrect ? scoreSettings.TotalPoints : scoreSettings.IncorrectPoint.GetValueOrDefault() * -1;

    public static int GetPoint(this MultipleChoiceScoreSettings scoreSettings, IEnumerable<Answer> correctedAnswers, IEnumerable<Answer> userCorrected)
    {
        var totalPoints = userCorrected.Sum(c => c.AnswerPoint);

        // Check fully correct.
        if (userCorrected.Count() == correctedAnswers.Count())
        {
            return totalPoints + scoreSettings.BonusPoints.GetValueOrDefault();
        }

        //todo: check against testportal, there is option to - point for each partial answer or whole.
        // Check partial correct allowed.
        return scoreSettings.IsPartialAnswersEnabled ? totalPoints : 0;
    }
}