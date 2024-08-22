namespace VietGeeks.TestPlatform.TestRunner.Contract
{
    public interface IActiveQuestion
    {
        public ExamQuestion? ActiveQuestion { get; set; }
        public int? ActiveQuestionIndex { get; set; }
    }
}