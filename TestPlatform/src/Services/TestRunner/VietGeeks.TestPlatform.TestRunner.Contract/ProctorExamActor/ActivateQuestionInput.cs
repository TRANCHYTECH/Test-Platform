namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;


public class ActivateNextQuestionInput
{
    public string CurrentQuestionId {get;set;} = string.Empty;
    public ActivateDirection Direction {get;set;}
}

public enum ActivateDirection {
    Previous,
    Next
}