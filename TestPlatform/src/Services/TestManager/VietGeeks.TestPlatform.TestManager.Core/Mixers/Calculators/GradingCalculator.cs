using System;
using System.Collections.Generic;
using System.Linq;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Core.ReadonlyModels;

namespace VietGeeks.TestPlatform.TestManager.Core.Logics;

public static class GradingCalculator
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

                var aggregatedGrading = new AggregatedGrading()
                {
                    PassMarkGrade = new PassMarkGrade
                    {
                        FinalPoints = v,
                        IsPass = v >= passMaskCriteria.Value,
                        TotalPoints = totalPoints,
                        Unit = passMaskCriteria.Unit
                    },
                    GradingType = GradingCriteriaConfigType.PassMask
                };

                result.Add(aggregatedGrading);
            }
            else if (setting.Value is GradeRangeCriteria gradeRangeCriteria)
            {
                var v = finalMark;
                if (gradeRangeCriteria.Unit == RangeUnit.Percent)
                {
                    v = (finalMark / totalPoints) * 100;
                }

                var matchedLevel = gradeRangeCriteria.Details.OrderBy(c => c.To).First(c => c.To >= v);
                var aggregatedGrading = new AggregatedGrading()
                {
                    Grades = matchedLevel.Grades,
                    GradingType = GradingCriteriaConfigType.GradeRanges,
                };

                result.Add(aggregatedGrading);
            }
        }

        return result;
    }
}