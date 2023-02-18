using System.Collections.Generic;

namespace VietGeeks.TestPlatform.TestManager.Contract
{
    public class NewQuestionViewModel
    {
        public int QuestionNo { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public AnswerType AnswerType { get; set; }
        public IEnumerable<AnswerViewModel> Answers { get; set; }
        public ScoreSettingsViewModel ScoreSettings { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class QuestionViewModel
    {
        public string Id { get; set; }
        public int QuestionNo { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? CategoryColor { get; set; }
        public AnswerType AnswerType { get; set; }
        public IEnumerable<AnswerViewModel> Answers { get; set; }
        public ScoreSettingsViewModel ScoreSettings { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class ScoreSettingsViewModel
    {
        public int CorrectPoint { get; set; }
        public int IncorrectPoint { get; set; }
        public bool IsPartialAnswersEnabled { get; set; }
        public int MaxPoints { get; set; }
        public int MaxWords { get; set; }
    }

    public class AnswerViewModel
    {
        public string AnswerDescription { get; set; }
        public int AnswerPoint { get; set; }
        public bool IsCorrect { get; set; }
    }
}
