using System;
namespace VietGeeks.TestPlatform.TestRunner.Contract
{
    public class ExamContentOutput
    {
        public ExamQuestion[] Questions { get; set; } = default!;
    }

    public class ExamQuestion
    {
        public string Id { get; set; } = default!;

        public string Description { get; set; } = default!;

        public int AnswerType { get; set; }

        public ExamAnswer[] Answers { get; set; } = default!;
    }

    public class ExamAnswer
    {
        public string Id { get; set; } = default!;

        public string Description { get; set; } = default!;
    }
}

