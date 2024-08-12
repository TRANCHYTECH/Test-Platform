namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

public class AfterTestConfigOutput
{
    public TestEndConfigOutput? TestEndConfig { get; set; }
    public InformRespondentConfigOutput? InformRespondentConfig { get; set; }
}

public class TestEndConfigOutput
{
    public string? Message { get; set; }

    public bool RedirectTo { get; set; }

    public string? ToAddress { get; set; }
}

public class InformRespondentConfigOutput
{
    public bool InformViaEmail { get; set; }

    public string? PassedMessage { get; set; }

    public string? FailedMessage { get; set; }

    public Dictionary<string, bool> InformFactors { get; set; } = new Dictionary<string, bool>();
}