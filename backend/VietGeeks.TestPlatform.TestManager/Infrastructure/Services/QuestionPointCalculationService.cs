using VietGeeks.TestPlatform.TestManager.Data.Models;
using AnswerType = VietGeeks.TestPlatform.TestManager.Data.Models.AnswerType;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

internal class QuestionPointCalculationService : IQuestionPointCalculationService
{
    public int CalculateTotalPoints(QuestionDefinition question)
    {
        if (question.ScoreSettings == null)
        {
            return 0;
        }

        return CalculateTotalPointsByAnswerType(question);
    }

    private int CalculateTotalPointsByAnswerType(QuestionDefinition questionViewModel)
    {
        switch (questionViewModel.AnswerType)
        {
            case AnswerType.SingleChoice:
                return CalculateSingleChoiceTotalPoints((SingleChoiceScoreSettings)questionViewModel.ScoreSettings);
            case AnswerType.MultipleChoice:
                return CalculateMultipleChoiceTotalPoints(questionViewModel.Answers,
                    (MultipleChoiceScoreSettings)questionViewModel.ScoreSettings);
            case AnswerType.TrueFalse:
            case AnswerType.ShortAnswer:
            default:
                return 0;
        }
    }

    private int CalculateSingleChoiceTotalPoints(SingleChoiceScoreSettings scoreSettings)
    {
        return scoreSettings.CorrectPoint;
    }

    private int CalculateMultipleChoiceTotalPoints(IEnumerable<Answer> answers,
        MultipleChoiceScoreSettings scoreSettings)
    {
        if (scoreSettings.IsPartialAnswersEnabled)
        {
            var points = 0;

            if (answers?.Count() > 0)
            {
                points = answers.Sum(a => a.AnswerPoint);
            }

            var bonusPoint = scoreSettings.BonusPoints ?? 0;
            points += bonusPoint;

            return points;
        }

        return scoreSettings.CorrectPoint;
    }
}