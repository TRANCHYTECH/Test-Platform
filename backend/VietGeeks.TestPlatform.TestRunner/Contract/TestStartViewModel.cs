using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestRunner.Contract;

public class TestSession
{
    public string TestRunId { get; set; } = default!;

    public string AccessCode { get; set; } = default!;

    public ExamStep PreviousStep { get; set; }

    public string ClientProof { get; set; } = default!;

    public string ProctorExamId { get; set; } = default!;

    public string ExamId { get; set; } = default!;

    [JsonIgnore] public TimeSpan LifeTime { get; set; }
}

public class VerifyTestInput
{
    public string? TestId { get; set; }

    public string? AccessCode { get; set; }
}

public class VerifyTestOutputViewModel
{
    public string AccessCode { get; set; } = default!;
    public string TestName { get; set; } = default!;
    public string? ConsentMessage { get; set; } = default!;
    public string? InstructionMessage { get; set; } = default!;
}

public class VerifyTestOutput
{
    public string TestRunId { get; set; } = default!;

    public string TestName { get; set; } = default!;

    public string AccessCode { get; set; } = default!;

    public string? InstructionMessage { get; set; }

    public string? ConsentMessage { get; set; }

    public DateTime StartAtUtc { get; set; }

    public DateTime EndAtUtc { get; set; }

    public string ProctorExamId { get; set; } = default!;
}

public class SubmitAnswerViewModel
{
    public string QuestionId { get; set; } = default!;

    public string[] AnswerIds { get; set; } = default!;
}

public class SubmitAnswerOutputViewModel : IExamStepInfo
{
    public ExamQuestion? ActiveQuestion { get; set; } = default!;
    public ExamStep Step { get; set; }
}

public class ProvideExamineeInfoViewModel
{
    public IDictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
}

public class ProvideExamineeInfoInput : ProvideExamineeInfoViewModel
{
    public string TestRunId { get; set; } = default!;
    public string AccessCode { get; set; } = default!;
}

public class ProvideExamineeInfoOutput : IExamStepInfo
{
    public ExamStep Step { get; set; }
}

public class GenerateExamContentInput
{
    public string TestDefinitionId { get; set; } = default!;
    public string? AccessCode { get; set; }
    public string ExamId { get; set; } = default!;
    public DateTime StartedAt { get; set; }
}

public class FinishExamInput
{
    public IDictionary<string, string[]> Answers = default!;
    public string ExamId { get; set; } = default!;
    public DateTime FinishededAt { get; set; }
    public DateTime StartedAt { get; set; }
    public IDictionary<string, DateTime?[]> QuestionTimes { get; set; } = default!;
    public TimeSpan TotalDuration { get; set; }
}