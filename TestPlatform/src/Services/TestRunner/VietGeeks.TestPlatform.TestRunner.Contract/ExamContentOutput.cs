using System;
namespace VietGeeks.TestPlatform.TestRunner.Contract;

public class ExamContentOutput
{
    public ExamQuestion[] Questions { get; set; } = default!;

    public TestDuration TestDuration { get; set; } = default!;

    public bool CanSkipQuestion { get; set; }
    public ExamQuestion? ActiveQuestion { get; set; } = default!;
}

public class TestDuration
{
    public TestDurationMethodType Method { get; set; }

    public TimeSpan Duration { get; set; }
}

//todo: duplicate it.
public enum TestDurationMethodType
{
    CompleteTestTime = 1,
    CompleteQuestionTime = 2
}

public class ExamQuestion
{
    public string Id { get; set; } = default!;

    public string Description { get; set; } = default!;

    public int AnswerType { get; set; }

    public ExamAnswer[] Answers { get; set; } = default!;

    public ExamQuestionScoreSettings? ScoreSettings {get;set;}
}

public class ExamAnswer
{
    public string Id { get; set; } = default!;

    public string Description { get; set; } = default!;
}

public class ExamQuestionScoreSettings
{
    public int TotalPoints { get; set; }
    public bool IsDisplayMaximumScore { get; set; }
    public bool MustAnswerToContinue { get; set; }
    public bool MustCorrect { get; set; }
}