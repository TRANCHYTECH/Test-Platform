namespace VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor
{
    public class SubmitAnswerOutput
    {
        public bool IsSuccess { get; set; }
        public string? Reason { get; set; }
        public bool Terminated { get; set; }
    }
}