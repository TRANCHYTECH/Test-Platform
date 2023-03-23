using AutoMapper;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core;
using VietGeeks.TestPlatform.TestManager.Core.Logics;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;
using VietGeeks.TestPlatform.TestRunner.Infrastructure.Services;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

public class ProctorService : IProctorService
{
    private readonly IMapper _mapper;
    private readonly ITime _time;

    public ProctorService(IMapper mapper, ITime time)
    {
        _mapper = mapper;
        _time = time;
    }

    //todo: PLAN - need to refactor this class, instead of directly access to original database, it should be a separate microservice.
    // For verifying test, still use test definition instead of test run.
    public async Task<VerifyTestOutput> VerifyTest(VerifyTestInput input)
    {
        TestDefinition? matchedTestDef = null;
        string accessCode = string.Empty;
        if (!string.IsNullOrEmpty(input.TestId))
        {
            matchedTestDef = await DB.Find<TestDefinition>().MatchID(input.TestId).ExecuteFirstAsync();
            accessCode = Guid.NewGuid().ToString();
        }
        else if (!string.IsNullOrEmpty(input.AccessCode))
        {
            var getTestQuery = new Template<TestDefinition>(@"
                [
                    {
                        '$match': {
                            'TestAccessSettings.Settings._t': 'PrivateAccessCodeType', 
                            'TestAccessSettings.Settings.Configs.Code': '<access_code>'
                        }
                    }
                ]
            ").Tag("access_code", input.AccessCode);
            matchedTestDef = await DB.PipelineSingleAsync(getTestQuery);
            accessCode = input.AccessCode;
        }

        if (matchedTestDef == null || string.IsNullOrEmpty(accessCode))
        {
            throw new TestPlatformException("Not found test with access code");
        }

        // Check if test is activated/scheduled, and on time.
        if (!matchedTestDef.TestInRunning())
        {
            throw new TestPlatformException("The test is not activated/scheduled or ended");
        }

        // Get test run
        var testRun = await DB.Find<TestRun>().MatchID(matchedTestDef.CurrentTestRun?.Id).ExecuteSingleAsync();
        if (testRun == null)
        {
            throw new TestPlatformException("Not found test run for the test definition");
        }

        // Check period time of test run is still valid.
        var checkMoment = _time.UtcNow();
        if (testRun.ExplicitEnd || checkMoment > testRun.EndAtUtc)
        {
            throw new TestPlatformException("The test is already ended");
        }

        if (checkMoment < testRun.StartAtUtc)
        {
            throw new TestPlatformException("The test is not started yet");
        }

        return testRun.ToOutput(matchedTestDef, accessCode);
    }

    public async Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input)
    {
        var exam = await GetExam(input.TestRunId, input.AccessCode);
        if (exam == null)
        {
            exam = new Exam
            {
                TestRunId = input.TestRunId,
                AccessCode = input.AccessCode,
                ExamineeInfo = input.ExamineeInfo
            };

            await DB.InsertAsync(exam);
        }

        return exam.ID;
    }

    public async Task<ExamContentOutput> GenerateExamContent(GenerateExamContentInput input)
    {
        // Ensure exam is processed by order by checking if exam already created from step providing examinee info.
        var exam = await EnsureExam(input.ExamId);
        if (exam.Questions == null || !exam.Questions.Any())
        {
            var testRun = await DB.Find<TestRun>().Match(exam.TestRunId).ExecuteSingleAsync();
            var testDefinition = testRun.TestDefinitionSnapshot;
            //todo: discuss based on test portal. That can edit time settings and other parts. currently assume all info not changed when activated.
            var timeSettings = testDefinition.TimeSettings ?? throw new Exception("TimeSettings not configured properly");
            var gradingSettings = testDefinition.GradingSettings ?? throw new Exception("GradingSettings not configured properly");

            // Get questions of test run.
            var questions = await GetTestRunQuestions(testRun.ID);

            //todo: only store brief info of questions.
            exam.Questions = testDefinition.GenerateTestSet(questions, exam.AccessCode);
            //todo: check if no need to store, query when finishing.
            exam.TimeSettings = timeSettings;
            exam.GradeSettings = gradingSettings;
            
            await DB.SaveAsync(exam);
        }

        return exam.ToOutput();
    }

    public async Task<FinishExamOutput> FinishExam(FinishExamInput input)
    {
        var exam = await EnsureExam(input.ExamId);
        exam.StartedAt = input.StartedAt;
        exam.FinishedAt = input.FinishededAt;
        exam.FinalMark = CalculateExamMark(exam, input.Answers);
        //todo: check way to get total points of question, because there is field Bonous Point.
        exam.Grading = exam.GradeSettings.CalculateGrading(exam.FinalMark, exam.Questions.Sum(c => c.ScoreSettings.TotalPoints));
        // Calculate pass grading.
        await DB.SaveOnlyAsync(exam, new[] { nameof(Exam.StartedAt), nameof(Exam.FinishedAt), nameof(Exam.FinalMark) });

        return new FinishExamOutput
        {
            FinalMark = exam.FinalMark,
            Grading = _mapper.Map<List<AggregatedGrading>>(exam.Grading)
        };
    }

    private async Task<Exam> GetExam(string testRunId, string accessCode)
    {
        return await DB.Find<Exam>().Match(c => c.TestRunId == testRunId && c.AccessCode == accessCode).ExecuteSingleAsync();
    }

    private async Task<Exam> GetExam(string examId)
    {
        return await DB.Find<Exam>().MatchID(examId).ExecuteSingleAsync();
    }

    private async Task<Exam> EnsureExam(string examId)
    {
        return await GetExam(examId) ?? throw new TestPlatformException("NotFoundExam");
    }

    private async Task<List<QuestionDefinition>> GetTestRunQuestions(string testRunId)
    {
        var batches = await DB.Find<TestRunQuestion>().ManyAsync(c => c.TestRunId == testRunId);

        return batches.SelectMany(c => c.Batch).ToList();
    }

    private async Task<TestDefinition> GetTestDefinition(string testDefinitionId)
    {
        return await DB.Find<TestDefinition>().MatchID(testDefinitionId).ExecuteSingleAsync() ?? throw new TestPlatformException("NotFoundTestDefinition");
    }

    private static int CalculateExamMark(Exam exam, Dictionary<string, string[]> answers)
    {
        int finalMark = 0;
        foreach (var question in exam.Questions)
        {
            string[]? answer;
            answers.TryGetValue(question.ID, out answer);
            finalMark += question.CalculateMark(answer);
        }

        return finalMark;
    }
}
