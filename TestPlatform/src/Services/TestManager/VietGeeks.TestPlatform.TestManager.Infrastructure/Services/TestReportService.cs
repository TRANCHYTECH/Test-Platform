using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract.Exam;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

public interface ITestReportService
{
    Task<List<TestRunSummary>> GetTestRunSummaries(string testId);
    Task<List<ExamSummary>> GetExamSummaries(string[] testRunIds);
    Task<List<Respondent>> GetRespondents(string[] testRunIds);
    Task<ExamReview> LoadExamReview(string examId);
}

public class TestReportService : ITestReportService
{
    private readonly TestManagerDbContext _managerDbContext;

    public TestReportService(TestManagerDbContext managerDbContext)
    {
        _managerDbContext = managerDbContext;
    }

    public async Task<List<ExamSummary>> GetExamSummaries(string[] testRunIds)
    {
        // Verify that test runs belongs to user.
        var testRuns = await _managerDbContext.Find<TestRun>().Match(c => testRunIds.Contains(c.ID)).Project(c => new TestRun { ID = c.ID }).ExecuteAsync();
        if (testRuns.Count != testRunIds.Count())
        {
            throw new TestPlatformException("Invalid Test Run Ids");
        }

        var examEntities = await _managerDbContext.Find<Exam>()
            .IgnoreGlobalFilters()
            .Match(c => testRunIds.Contains(c.TestRunId))
            .Project(c => new Exam
            {
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
        var testRuns = await _managerDbContext.Find<TestRun>()
            .Match(c => testRunIds.Contains(c.ID))
            .Project(c => new TestRun { ID = c.ID })
            .ExecuteAsync();

        if (testRuns.Count != testRunIds.Count())
        {
            throw new TestPlatformException("Invalid Test Run Ids");
        }

        var examEntities = await _managerDbContext.Find<Exam>()
            .IgnoreGlobalFilters()
            .Match(c => testRunIds.Contains(c.TestRunId))
            .Sort(c => c.StartedAt, Order.Descending)
            .Project(c => new Exam
            {
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

    public async Task<ExamReview> LoadExamReview(string examId)
    {
        var examEntity = await _managerDbContext.Find<Exam>()
                   .IgnoreGlobalFilters()
                   .MatchID(examId)
                   .ExecuteFirstAsync();

        if (examEntity == null)
        {
            throw new TestPlatformException("Not found exam");
        }

        var batches = await DB.Find<TestRunQuestion>().ManyAsync(c => c.TestRunId == examEntity.TestRunId);
        var questions = batches.SelectMany(c => c.Batch);

        return new()
        {
            // Set questions
            // Set answers
            // Set scores
        };
    }


    public async Task<List<TestRunSummary>> GetTestRunSummaries(string testId)
    {
        var testRunEntities = await _managerDbContext.Find<TestRun>().Match(c => c.TestDefinitionSnapshot.ID == testId).ExecuteAsync();
        return testRunEntities.Select(c => new TestRunSummary
        {
            Id = c.ID,
            StartAt = c.StartAtUtc,
            EndAt = c.EndAtUtc
        }).ToList();
    }

    private string GetExamInfoField(Exam exam, string fieldName)
    {
        return exam.ExamineeInfo.ContainsKey(fieldName) ? exam.ExamineeInfo[fieldName] : string.Empty;
    }
}

