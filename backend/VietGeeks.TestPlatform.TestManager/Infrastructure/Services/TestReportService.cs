using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract.Exam;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public interface ITestReportService
{
    Task<List<TestRunSummary>> GetTestRunSummaries(string testId);
    Task<List<ExamSummary>> GetExamSummaries(string[] testRunIds);
    Task<List<Respondent>> GetRespondents(string[] testRunIds);
    Task<ExamReview> GetExamReview(string examId);
}

public class TestReportService(
    TestManagerDbContext managerDbContext,
    IQuestionCategoryService questionCategoryService)
    : ITestReportService
{
    public async Task<List<ExamSummary>> GetExamSummaries(string[] testRunIds)
    {
        // Verify that test runs belongs to user.
        var testRuns = await managerDbContext.Find<TestRun>().Match(c => testRunIds.Contains(c.ID))
            .Project(c => new TestRun { ID = c.ID }).ExecuteAsync();
        if (testRuns.Count != testRunIds.Count())
        {
            throw new TestPlatformException("Invalid Test Run Ids");
        }

        var examEntities = await managerDbContext.Find<Exam>()
            .IgnoreGlobalFilters()
            .Match(c => testRunIds.Contains(c.TestRunId))
            .Project(c => new Exam
            {
                ID = c.ID,
                ExamineeInfo = c.ExamineeInfo,
                FinalMark = c.FinalMark,
                StartedAt = c.StartedAt,
                FinishedAt = c.FinishedAt,
                TotalTime = c.TotalTime
            })
            .ExecuteAsync();

        //todo: Capitalize examinee info
        const string format = @"hh\:mm\:ss";
        return examEntities.Select(exam =>
        {
            return new ExamSummary
            {
                Id = exam.ID,
                FirstName = GetExamInfoField(exam, "firstName"),
                LastName = GetExamInfoField(exam, "lastName"),
                FinalMark = exam.FinalMark,
                StartedAt = exam.StartedAt,
                FinishedAt = exam.FinishedAt,
                TotalTime = exam.TotalTime.ToString(format)
            };
        }).ToList();
    }

    public async Task<List<Respondent>> GetRespondents(string[] testRunIds)
    {
        // Verify that test runs belongs to user.
        var testRuns = await managerDbContext.Find<TestRun>()
            .Match(c => testRunIds.Contains(c.ID))
            .Project(c => new TestRun { ID = c.ID })
            .ExecuteAsync();

        if (testRuns.Count != testRunIds.Count())
        {
            throw new TestPlatformException("Invalid Test Run Ids");
        }

        var examEntities = await managerDbContext.Find<Exam>()
            .IgnoreGlobalFilters()
            .Match(c => testRunIds.Contains(c.TestRunId))
            .Sort(c => c.StartedAt, Order.Descending)
            .Project(c => new Exam
            {
                ID = c.ID,
                ExamineeInfo = c.ExamineeInfo
            })
            .ExecuteAsync();

        //todo: Capitalize examinee info
        return examEntities.Select(exam =>
        {
            return new Respondent
            {
                ExamId = exam.ID,
                FirstName = GetExamInfoField(exam, "firstName"),
                LastName = GetExamInfoField(exam, "lastName")
            };
        }).ToList();
    }

    //todo: build report into each readonly view in order to save server resources. but disadvangte of this approach is that if review template has break changes. suggestion: create report version
    public async Task<ExamReview> GetExamReview(string examId)
    {
        var examEntity = await managerDbContext.Find<Exam>()
            .IgnoreGlobalFilters()
            .MatchID(examId)
            .ExecuteFirstAsync() ?? throw new TestPlatformException("Not found exam");

        var examQuestions = await GetTestRunQuestions(examEntity);

        //todo: IMPORTANT -improve architecture design for query. store hierchacy data from testdef => test run => exam, tenant to query easily.
        var testRun = await managerDbContext.Find<TestRun>().MatchID(examEntity.TestRunId)
            .Project(c => new TestRun { ID = c.ID, TestDefinitionSnapshot = c.TestDefinitionSnapshot })
            .ExecuteFirstAsync();
        //todo: cache question categories within original method?
        var questionCategories =
            await questionCategoryService.GetCategories(testRun.TestDefinitionSnapshot.ID, default);
        return new ExamReview
        {
            StartedAt = examEntity.StartedAt,
            FinishedAt = examEntity.FinishedAt,
            ActualTotalDuration = examEntity.TotalTime.ToString(@"hh\:mm\:ss"),
            TotalDuration = examEntity.TotalDuration.ToString(@"hh\:mm\:ss"),
            FirstName = GetExamInfoField(examEntity, "firstName"),
            LastName = GetExamInfoField(examEntity, "lastName"),
            Questions = examQuestions.Select(q => new
            {
                Id = q.ID,
                q.Description,
                q.Answers,
                q.AnswerType,
                q.CategoryId,
                CategoryName = questionCategories.Single(c => c.Id == q.CategoryId).Name,
                q.ScoreSettings.TotalPoints,
                ActualPoints = GetActualScores(examEntity, q.ID),
                AnswerTime = GetAnswerTime(examEntity, q.ID)
            }),
            Answers = examEntity.Answers,
            Scores = examQuestions.GroupBy(c => c.CategoryId).Select(c => new
            {
                CategoryId = c.Key,
                CategoryName = questionCategories.Single(d => d.Id == c.Key).Name,
                NumberOfQuestions = c.Count(),
                TotalPoints = c.Sum(q => q.ScoreSettings.TotalPoints),
                ActualPoints = c.Select(d => GetActualScores(examEntity, d.ID)).Sum(score => score)
            }),
            Grading = examEntity.Grading
        };
    }

    public async Task<List<TestRunSummary>> GetTestRunSummaries(string testId)
    {
        var testRunEntities = await managerDbContext.Find<TestRun>()
            .Match(c => c.TestDefinitionSnapshot.ID == testId).ExecuteAsync();
        return testRunEntities.Select(c => new TestRunSummary
        {
            Id = c.ID,
            StartAt = c.StartAtUtc,
            EndAt = c.EndAtUtc
        }).ToList();
    }

    //todo: refactor this, move to extension methods of Exam Entity.
    private string GetExamInfoField(Exam exam, string fieldName)
    {
        return exam.ExamineeInfo.ContainsKey(fieldName) ? exam.ExamineeInfo[fieldName] : string.Empty;
    }

    private async Task<IEnumerable<QuestionDefinition>> GetTestRunQuestions(Exam examEntity)
    {
        var questionBatches = await managerDbContext.Find<TestRunQuestion>().IgnoreGlobalFilters()
            .ManyAsync(c => c.TestRunId == examEntity.TestRunId);
        var testRunQuestions = questionBatches.SelectMany(c => c.Batch);
        var examQuestions = examEntity.Questions.Select(id => testRunQuestions.Single(q => q.ID == id));

        return examQuestions;
    }

    private int GetActualScores(Exam exam, string questionId)
    {
        if (exam.QuestionScores.TryGetValue(questionId, out var point))
        {
            return point;
        }

        return 0;
    }

    private string GetAnswerTime(Exam exam, string questionId)
    {
        var result = TimeSpan.FromSeconds(0);

        if (exam.QuestionTimes.TryGetValue(questionId, out var period))
        {
            result = period[1]!.Value.Subtract(period[0]!.Value);
        }

        return result.ToString(@"mm\:ss");
    }
}