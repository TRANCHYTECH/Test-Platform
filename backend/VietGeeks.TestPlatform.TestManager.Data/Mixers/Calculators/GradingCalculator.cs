using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Data.ReadonlyModels;

namespace VietGeeks.TestPlatform.TestManager.Data.Mixers.Calculators
{
    public static class GradingCalculator
    {
        public static List<AggregatedGrading> CalculateGrading(this GradingSettingsPart settings, decimal finalMark,
            decimal totalPoints)
        {
            var result = new List<AggregatedGrading>();
            foreach (var setting in settings.GradingCriterias)
            {
                if (setting.Value is PassMaskCriteria passMaskCriteria)
                {
                    var v = finalMark;
                    if (passMaskCriteria.Unit == RangeUnit.Percent)
                    {
                        v = finalMark / totalPoints * 100;
                    }

                    var aggregatedGrading = new AggregatedGrading
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
                        v = finalMark / totalPoints * 100;
                    }

                    var matchedLevel = gradeRangeCriteria.Details.OrderBy(c => c.To).First(c => c.To >= v);
                    var aggregatedGrading = new AggregatedGrading
                    {
                        Grades = matchedLevel.Grades,
                        GradingType = GradingCriteriaConfigType.GradeRanges
                    };

                    result.Add(aggregatedGrading);
                }
            }

            return result;
        }
    }
}