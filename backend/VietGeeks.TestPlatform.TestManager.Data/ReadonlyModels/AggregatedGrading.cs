using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Data.ReadonlyModels
{
    public class AggregatedGrading
    {
        public GradingCriteriaConfigType GradingType { get; set; }
        public IDictionary<string, string>? Grades { get; set; }
        public PassMarkGrade? PassMarkGrade { get; set; }
    }

    public class PassMarkGrade
    {
        public bool? IsPass { get; set; }
        public decimal? FinalPoints { get; set; }
        public decimal? TotalPoints { get; set; }
        public decimal? PassValue { get; set; }
        public RangeUnit Unit { get; set; }
    }
}