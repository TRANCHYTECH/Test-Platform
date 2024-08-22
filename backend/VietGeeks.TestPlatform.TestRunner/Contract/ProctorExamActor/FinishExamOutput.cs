namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class FinishExamOutput
{
    public decimal FinalMark { get; set; }
    public List<AggregatedGradingOuput> Grading { get; set; } = default!;
    public DateTime FinishedAt { get; set; }
    public TimeSpan TotalTime { get; set; }
    public IEnumerable<QuestionOutput> Questions { get; set; }
    public IDictionary<string, string[]> ExamAnswers { get; set; }

    public IDictionary<string, int> QuestionScores { get; set; } = default!;
}

public class AggregatedGradingOuput
{
    public int GradingType { get; set; }
    public PassMarkGradeOutput? PassMarkGrade { get; set; }
    public IDictionary<string, string>? Grades { get; set; }
}

public class PassMarkGradeOutput
{
    public bool? IsPass { get; set; }
    public decimal? FinalPoints { get; set; }
    public decimal? TotalPoints { get; set; }
    public decimal? PassValue { get; set; }
    public int Unit { get; set; }
}

public class QuestionOutput
{
    public string Id { get; set; }
    public int QuestionNo { get; set; }
    public string Description { get; set; }
    public string CategoryId { get; set; }
    public int AnswerType { get; set; }
    public AnswerOutput[] QuestionAnswers { get; set; }
    public int TotalPoints { get; set; }
}

public class AnswerOutput
{
    public string Id { get; set; } = default!;
    public string AnswerDescription { get; set; } = default!;
    public bool IsCorrect { get; set; }
}