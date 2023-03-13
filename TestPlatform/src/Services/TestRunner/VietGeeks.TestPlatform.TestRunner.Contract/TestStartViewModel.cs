using System;
using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestRunner.Contract;


public class TestStartContinousToken
{
    //todo: rename to test defintion id.
    public string TestId { get; set; } = default!;

    public string AccessCode { get; set; } = default!;

    public PreStartSteps PreviousStep { get; set; }

    public string ClientFootage { get; set; } = default!;
    public string ExamId { get; set; }
}

public class VerifyTestViewModel
{
    public string? TestId { get; set; }

    public string? AccessCode { get; set; }
}

public class VerifyTestResultViewModel
{
    public string TestId { get; set; } = default!;

    public string AccessCode { get; set; } = default!;

    public bool IsValid { get; set; }
}

public class ProvideExamineeInfoViewModel
{
    public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
}

public class ProvideExamineeInfoInputViewModel
{
    public string TestId { get; set; } = default!;

    public string AccessCode { get; set; } = default!;

    public Dictionary<string, string> ExamineeInfo { get; set; } = new Dictionary<string, string>();
}

public enum PreStartSteps
{
    Verify = 1,
    ProvideExamineeInfo = 2
}

public class GenerateExamContentInput
{
    public string TestDefinitionId { get; set; } = default!;

    public string? AccessCode { get; set; }
}