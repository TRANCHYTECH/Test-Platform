using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

public static class ExamMapper
{
    public static ExamQuestion ToViewModel(this QuestionDefinition model)
    {
        return new ExamQuestion
        {
            Id = model.ID,
            Description = model.Description,
            AnswerType = model.AnswerType,
            Answers = model.Answers.Select(c => new ExamAnswer
            {
                Id = c.Id,
                Description = c.AnswerDescription,
            }).ToArray()
        };
    }

    public static TestDuration ToViewModel(this TestDurationMethod model)
    {
        if (model is CompleteQuestionDuration questionDuration)
        {
            return new()
            {
                Method = TestDurationMethodType.CompleteQuestionTime,
                Duration = questionDuration.Duration
            };
        }

        if (model is CompleteTestDuration testDuration)
        {
            return new()
            {
                Method = TestDurationMethodType.CompleteTestTime,
                Duration = testDuration.Duration
            };
        }

        throw new Exception("Not supported type");
    }
}