using System.Text.Json.Serialization;

namespace VietGeeks.TestPlatform.TestManager.Contract
{
    public class CreateOrUpdateQuestionViewModel : IQuestionViewModel
    {
        public string Description { get; set; } = default!;
        public string CategoryId { get; set; } = default!;
        public AnswerType AnswerType { get; set; }
        public IEnumerable<AnswerViewModel> Answers { get; set; } = default!;
        public ScoreSettingsViewModel ScoreSettings { get; set; } = default!;
        public bool IsMandatory { get; set; }
    }

    public class QuestionViewModel : IQuestionViewModel
    {
        public string Id { get; set; } = default!;
        public int QuestionNo { get; set; }
        public string Order { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string CategoryId { get; set; } = default!;
        public string? CategoryName { get; set; }
        public string? CategoryColor { get; set; }
        public AnswerType AnswerType { get; set; }
        public IEnumerable<AnswerViewModel> Answers { get; set; } = default!;
        public ScoreSettingsViewModel ScoreSettings { get; set; } = default!;
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
        public string Id { get; set; } = default!;
        public string AnswerDescription { get; set; } = default!;
        public int AnswerPoint { get; set; }
        public bool IsCorrect { get; set; }
    }

    public class QuestionOrderViewViewModel
    {
        public string Id { get; set; } = default!;
        public string Question { get; set; } = default!;
        public string Order { get; set; } = default!;
    }

    public class UpdateQuestionOrderViewModel
    {
        public string Id { get; set; } = default!;
        public string Order { get; set; } = default!;
    }
}
