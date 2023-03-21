using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract;

public class TestSession
{
    //todo: rename to test defintion id.
    public string TestId { get; set; } = default!;

    public string AccessCode { get; set; } = default!;

    public PreStartSteps PreviousStep { get; set; }

    public string ClientProof { get; set; } = default!;

    public string ExamId { get; set; } = default!;
}

public class VerifyTestInput
{
    public string? TestId { get; set; }

    public string? AccessCode { get; set; }
}

public class VerifyTestOutputViewModel
{
    public string TestName { get; set; } = default!;
    public string ConsentMessage { get; set; } = default!;
    public string InstructionMessage { get; set; } = default!;
}

public class VerifyTestResult
{
    public string TestId { get; set; } = default!;

    public string TestName { get; set; } = default!;

    public string AccessCode { get; set; } = default!;

    public string? InstructionMessage { get; set; }

    public string? ConsentMessage { get; set; }

    public bool IsValid { get; private set; }

    public static VerifyTestResult Invalid() => new VerifyTestResult
    {
        IsValid = false
    };

    public static VerifyTestResult Valid((string testId, string accessCode) data) => new()
    {
        TestId = data.testId,
        AccessCode = data.accessCode,
        IsValid = true
    };
}


public class SubmitAnswerViewModel
{
    public string QuestionId { get; set; } = default!;

    public string[] AnswerIds { get; set; } = default!;
}

public class ProvideExamineeInfoViewModel
{
    public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
}

public class ProvideExamineeInfoInput : ProvideExamineeInfoViewModel
{
    public string TestId { get; set; } = default!;

    public string? AccessCode { get; set; } = default;
}

public enum PreStartSteps
{
    VerifyTest = 1,
    ProvideExamineeInfo = 2,
    Start = 3,
    SubmitAnswer = 4,
    FinishExam = 5
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
    public string ExamId { get; set; } = default!;
    public DateTime FinishededAt { get; set; }
    public DateTime StartedAt { get; set; }

    public Dictionary<string, string[]> Answers = default!;
}

public class FinishExamOutputViewModel
{
    public decimal TotalPoints { get; set; }
}