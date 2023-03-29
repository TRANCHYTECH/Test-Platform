﻿using AutoMapper;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Core;
using VietGeeks.TestPlatform.TestManager.Core.Logics;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

public class ProctorService : IProctorService
{
    private readonly IMapper _mapper;
    private readonly IClock _time;

    public ProctorService(IMapper mapper, IClock time)
    {
        _mapper = mapper;
        _time = time;
    }

    //todo: PLAN - need to refactor this class, instead of directly access to original database, it should be a separate microservice.
    // For verifying test, still use test definition instead of test run.
    public async Task<VerifyTestOutput> VerifyTest(VerifyTestInput input)
    {
        if (string.IsNullOrEmpty(input.TestId) && string.IsNullOrEmpty(input.AccessCode))
        {
            throw new TestPlatformException("Invalid input");
        }

        TestDefinition? matchedTestDef = null;
        string accessCode = string.Empty;
        if (!string.IsNullOrEmpty(input.TestId))
        {
            var getTestQuery = new Template<TestDefinition>(@"
                [
                    {
                        '$match': {
                            '_id': <id>
                            'TestAccessSettings.Settings._t': 'PublicLinkType'
                        }
                    }
                ]
            ").Tag("id", $"ObjectId('{input.TestId}')");
            matchedTestDef = await DB.PipelineSingleAsync(getTestQuery);
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

        if (matchedTestDef == null)
        {
            throw new TestPlatformException("Not found test");
        }

        // Check if there is an existing exam created with this access code. Means it is used already.
        if (!string.IsNullOrEmpty(input.AccessCode))
        {
            if (await DB.CountAsync<Exam>(c => c.AccessCode == input.AccessCode) > 0)
            {
                throw new TestPlatformException("The access code is used");
            }
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
        var checkMoment = _time.UtcNow;
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
        if (exam != null)
        {
            throw new TestPlatformException("The examinee already provided info");
        }

        exam = new Exam
        {
            TestRunId = input.TestRunId,
            AccessCode = input.AccessCode,
            ExamineeInfo = input.ExamineeInfo
        };
        await DB.InsertAsync(exam);

        return exam.ID;
    }

    public async Task<ExamContentOutput> GenerateExamContent(GenerateExamContentInput input)
    {
        // Ensure exam is processed by order by checking if exam already created from step providing examinee info.
        //todo: 3 queries here, possible to improve.
        var exam = await EnsureExam(input.ExamId);
        var testRun = await DB.Find<TestRun>().Match(exam.TestRunId).ExecuteSingleAsync();
        var testDefinition = testRun.TestDefinitionSnapshot;
        var questions = await GetTestRunQuestions(testRun.ID);

        List<QuestionDefinition> selectedQuestions;
        if (exam.Questions == null || !exam.Questions.Any())
        {
            selectedQuestions = testDefinition.GenerateTestSet(questions, exam.AccessCode);
            exam.Questions = selectedQuestions.Select(c => c.ID).ToArray();

            await DB.SaveAsync(exam);
        }
        else
        {
            selectedQuestions = exam.Questions.Select(id => questions.Single(o => o.ID == id)).ToList();
        }

        return new()
        {
            Questions = selectedQuestions.Select(c => c.ToViewModel()).ToArray(),
            TestDuration = testDefinition.TimeSettings.TestDurationMethod.ToViewModel(),
            CanSkipQuestion = testDefinition.TimeSettings.AnswerQuestionConfig.SkipQuestion
        };
    }

    public async Task<FinishExamOutput> FinishExam(FinishExamInput input)
    {
        var exam = await EnsureExam(input.ExamId);
        //todo: 3 queries here, possible to improve. by temp store, then delete when finish. or scheduler. or cache tech.
        var testRun = await DB.Find<TestRun>().Match(exam.TestRunId).ExecuteSingleAsync();
        var testDefinition = testRun.TestDefinitionSnapshot;
        var questions = await GetTestRunQuestions(testRun.ID);
        var selectedQuestions = exam.Questions.Select(id => questions.Single(o => o.ID == id)).ToList();

        exam.Answers = input.Answers;
        exam.StartedAt = input.StartedAt;
        exam.FinishedAt = input.FinishededAt;
        exam.FinalMark = CalculateExamMark(selectedQuestions, input.Answers);

        exam.Grading = testDefinition.GradingSettings.CalculateGrading(exam.FinalMark, selectedQuestions.Sum(c => c.ScoreSettings.TotalPoints));

        var changedProps = new[] { nameof(Exam.Answers), nameof(Exam.StartedAt), nameof(Exam.FinishedAt), nameof(Exam.FinalMark), nameof(Exam.Grading) };
        await DB.SaveOnlyAsync(exam, changedProps);

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

    private static int CalculateExamMark(List<QuestionDefinition> questions, Dictionary<string, string[]> answers)
    {
        int finalMark = 0;
        foreach (var question in questions)
        {
            string[]? answer;
            answers.TryGetValue(question.ID, out answer);
            finalMark += question.CalculateMark(answer);
        }

        return finalMark;
    }
}