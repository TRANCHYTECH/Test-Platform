﻿using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Entities;

namespace VietGeeks.TestPlatform.TestManager.Core.Models
{
    [Collection("QuestionDefinition")]
    public class QuestionDefinition: EntityBase
    {
        public string TestId { get; set; }
        public int QuestionNo { get; set; }
        public string Description { get; set; }
        public string CategoryId { get; set; }
        public int AnswerType { get; set; }
        public Answer[] Answers { get; set; }
        public ScoreSettings ScoreSettings { get; set; }
        public bool IsMandatory { get; set; }
    }



    [BsonKnownTypes(typeof(SingleChoiceScoreSettings), typeof(MultipleChoiceScoreSettings))]
    public abstract class ScoreSettings
    {
        public int? TotalPoints { get; set; }
        public bool IsDisplayMaximumScore { get; set; }
        public bool MustAnswerToContinue { get; set; }
        public bool IsMandatory { get; set; }
    }

    public class SingleChoiceScoreSettings: ScoreSettings
    {
        public int? CorrectPoint { get; set; }
        public int? IncorrectPoint { get; set; }
    }

    public class MultipleChoiceScoreSettings : ScoreSettings
    {
        public int? CorrectPoint { get; set; }
        public int? IncorrectPoint { get; set; }
        public bool? IsPartialAnswersEnabled { get; set; }
        public int? BonusPoints { get; set; }
        public int? PartialIncorrectPoint { get; set; }
    }

    public class ShortAnswerScoreSettings : ScoreSettings
    {
        public int? MaxWords { get; set; }
    }

    public class Answer
    {
        public string Id { get; set; }
        public string AnswerDescription { get; set; }
        public int AnswerPoint { get; set; }
        public bool IsCorrect { get; set; }
    }
}
