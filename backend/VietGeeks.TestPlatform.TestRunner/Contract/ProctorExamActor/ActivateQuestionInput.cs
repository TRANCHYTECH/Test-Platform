namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor
{
    public class ActivateNextQuestionInput
    {
        public ActivateDirection Direction { get; set; }
    }

    public enum ActivateDirection
    {
        Previous,
        Next
    }
}