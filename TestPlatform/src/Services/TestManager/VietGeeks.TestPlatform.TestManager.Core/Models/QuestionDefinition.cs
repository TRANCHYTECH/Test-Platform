using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models
{
    [Collection("QuestionDefinition")]
    public class QuestionDefinition : EntityBase
    {
        public string TestId { get; set; } = default!;
        public int QuestionNo { get; set; }
        public string Description { get; set; } = default!;
        public string CategoryId { get; set; } = default!;
        public AnswerType AnswerType { get; set; }
        public Answer[] Answers { get; set; } = default!;
        public ScoreSettings ScoreSettings { get; set; } = default!;
        public bool IsMandatory { get; set; }
    }

    [BsonKnownTypes(typeof(SingleChoiceScoreSettings), typeof(MultipleChoiceScoreSettings))]
    public abstract class ScoreSettings
    {
        public int TotalPoints { get; set; }
        public bool IsDisplayMaximumScore { get; set; }
        public bool MustAnswerToContinue { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class SingleChoiceScoreSettings : ScoreSettings
    {
        public int CorrectPoint { get; set; }

        public int IncorrectPoint { get; set; }
    }

    public class MultipleChoiceScoreSettings : ScoreSettings
    {
        public int CorrectPoint { get; set; }
        public int IncorrectPoint { get; set; }
        public bool IsPartialAnswersEnabled { get; set; }
        public int? BonusPoints { get; set; }
        //todo: there are 2 options of this case. check testportal.
        public int? PartialIncorrectPoint { get; set; }
    }

    public class ShortAnswerScoreSettings : ScoreSettings
    {
        public int? MaxWords { get; set; }
    }

    public class Answer
    {
        public string Id { get; set; } = default!;
        public string AnswerDescription { get; set; } = default!;
        public int AnswerPoint { get; set; }
        public bool IsCorrect { get; set; }
    }

    public enum AnswerType
    {
        SingleChoice = 1,
        MultipleChoice = 2,
        TrueFalse = 3,
        ShortAnswer = 4
    }
}
