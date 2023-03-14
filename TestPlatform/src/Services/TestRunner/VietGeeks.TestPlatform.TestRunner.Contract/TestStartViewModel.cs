using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract;


public class TestSession
{
    //todo: rename to test defintion id.
    public string TestId { get; set; } = default!;

    public string? AccessCode { get; set; } = default!;

    public PreStartSteps PreviousStep { get; set; }

    public string ClientProof { get; set; } = default!;

    public string ExamId { get; set; } = default!;
}

public class VerifyTestInput
{
    public string? TestId { get; set; }

    public string? AccessCode { get; set; }
}

public class VerifyTestResultViewModel
{
    public string TestId { get; set; } = default!;

    public string? AccessCode { get; set; } = default!;

    public bool IsValid { get; private set; }

    public static VerifyTestResultViewModel Invalid() => new VerifyTestResultViewModel
    {
        IsValid = false
    };

    public static VerifyTestResultViewModel Valid(string testId) => new VerifyTestResultViewModel
    {
        TestId = testId,
        IsValid = true
    };

    public static VerifyTestResultViewModel Valid((string testId, string accessCode) data) => new VerifyTestResultViewModel
    {
        TestId = data.testId,
        AccessCode = data.accessCode,
        IsValid = true
    };
}

public class SubmitAnswerViewModel
{

}

public class ProvideExamineeInfoViewModel
{
    public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
}

public class ProvideExamineeInfoInput: ProvideExamineeInfoViewModel
{
    public string TestId { get; set; } = default!;

    public string? AccessCode { get; set; } = default;
}

public enum PreStartSteps
{
    Verified = 1,
    ProvidedExamineeInfo = 2,
    Started = 3
}

public class GenerateExamContentInput
{
    public string TestDefinitionId { get; set; } = default!;

    public string? AccessCode { get; set; }
}