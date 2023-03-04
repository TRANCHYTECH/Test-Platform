using VietGeeks.TestPlatform.TestManager.Contract;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    internal class QuestionPointCalculationService : IQuestionPointCalculationService
    {
        public void CalculateTotalPoints(IQuestionViewModel questionViewModel)
        {
            if (questionViewModel.ScoreSettings == null)
            {
                return;
            }

            questionViewModel.ScoreSettings.TotalPoints = CalculateTotalPointsByAnswerType(questionViewModel);
        }

        private int CalculateTotalPointsByAnswerType(IQuestionViewModel questionViewModel)
        {
            switch (questionViewModel.AnswerType)
            {
                case AnswerType.SingleChoice:
                    return CalculateSingleChoiceTotalPoints((SingleChoiceScoreSettingsViewModel)questionViewModel.ScoreSettings);
                case AnswerType.MultipleChoice:
                    return CalculateMultipleChoiceTotalPoints(questionViewModel.Answers, (MultipleChoiceScoreSettingsViewModel)questionViewModel.ScoreSettings);
                case AnswerType.TrueFalse:
                case AnswerType.ShortAnswer:
                default:
                    return 0;
            }
        }

        private int CalculateSingleChoiceTotalPoints(SingleChoiceScoreSettingsViewModel scoreSettings)
        {
            return scoreSettings.CorrectPoint ?? 0;
        }

        private int CalculateMultipleChoiceTotalPoints(IEnumerable<AnswerViewModel> answers, MultipleChoiceScoreSettingsViewModel scoreSettings)
        {
            if (scoreSettings.IsPartialAnswersEnabled == true)
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
            else
            {
                return scoreSettings.CorrectPoint ?? 0;
            }
        }
    }
}
