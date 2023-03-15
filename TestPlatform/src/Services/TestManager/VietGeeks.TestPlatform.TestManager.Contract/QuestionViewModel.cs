using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestManager.Contract
{
    public class NewQuestionViewModel: IQuestionViewModel
    {
        public int QuestionNo { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public AnswerType AnswerType { get; set; }
        public IEnumerable<AnswerViewModel> Answers { get; set; }
        public ScoreSettingsViewModel ScoreSettings { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class QuestionViewModel : IQuestionViewModel
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
    }

    [JsonDerivedType(typeof(SingleChoiceScoreSettingsViewModel), (int)AnswerType.SingleChoice)]
    [JsonDerivedType(typeof(MultipleChoiceScoreSettingsViewModel), (int)AnswerType.MultipleChoice)]
    public class ScoreSettingsViewModel
    {
        public int? TotalPoints { get; set; }
        public bool IsDisplayMaximumScore { get; set; }
        public bool MustAnswerToContinue { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class SingleChoiceScoreSettingsViewModel : ScoreSettingsViewModel
    {
        public int? CorrectPoint { get; set; }
        public int? IncorrectPoint { get; set; }
    }

    public class MultipleChoiceScoreSettingsViewModel : ScoreSettingsViewModel
    {
        public int? CorrectPoint { get; set; }
        public int? IncorrectPoint { get; set; }
        public bool IsPartialAnswersEnabled { get; set; } = false;
        public int? BonusPoints { get; set; }
        public int? PartialIncorrectPoint { get; set; }
    }

    public class AnswerViewModel
    {
        public string Id { get; set; }
        public string AnswerDescription { get; set; }
        public int AnswerPoint { get; set; }
        public bool IsCorrect { get; set; }
    }
}
