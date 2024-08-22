using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Data.Mixers.Calculators;

public class MultipleChoicesScoreCalculator : IScoreCalculator
{
    public bool IsCorrectAnswer(QuestionDefinition question, string[]? answerIds)
    {
        var (_, isFullyCorrect) = GetExamineeCorrectAnswers(question, answerIds);

        return isFullyCorrect;
    }

    public int Calculate(QuestionDefinition question, string[]? answerIds)
    {
        if (question.ScoreSettings is not MultipleChoiceScoreSettings scoreSettings)
        {
            throw new Exception("scoreSettings is not type of MultipleChoiceScoreSettings");
        }

        var (userCorrected, isFullyCorrect) = GetExamineeCorrectAnswers(question, answerIds);
        var totalPoints = 0;

        if (scoreSettings.IsPartialAnswersEnabled)
        {
            totalPoints = userCorrected.Sum(c => c.AnswerPoint);

            if (isFullyCorrect)
            {
                totalPoints += scoreSettings.BonusPoints.GetValueOrDefault();
            }

            return totalPoints;
        }

        if (isFullyCorrect)
        {
            totalPoints = scoreSettings.TotalPoints;
        }

        return totalPoints;
    }

    private static (IEnumerable<Answer>, bool) GetExamineeCorrectAnswers(QuestionDefinition question,
        string[]? answerIds)
    {
        var correctedAnswers = question.Answers.Where(c => c.IsCorrect);
        var examineeCorrectAnswers = answerIds == null
            ? Array.Empty<Answer>()
            : correctedAnswers.Where(c => answerIds.Contains(c.Id));
        var isFullyCorrect = examineeCorrectAnswers.Count() == correctedAnswers.Count();

        return (examineeCorrectAnswers, isFullyCorrect);
    }
}