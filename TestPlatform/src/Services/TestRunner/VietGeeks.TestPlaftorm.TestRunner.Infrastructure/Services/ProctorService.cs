using System.Text.Json;
using Dapr.Client;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core.Logics;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

public class ExamContentViewModel
{
    public string ExamId { get; set; } = default!;
}

public class ProctorService : IProctorService
{
    private readonly DaprClient _daprClient;

    public ProctorService(DaprClient daprClient)
    {
        _daprClient = daprClient;
    }

    public Task<ExamContentViewModel> GetExamContent()
    {
        return _daprClient.InvokeMethodAsync<ExamContentViewModel>(HttpMethod.Get, "proctor-manager", "Exam/Content");
    }

    //todo: need to refactor this class, instead of directly access to original database, it should be a separate microservice.
    public async Task<VerifyTestResult> VerifyTest(VerifyTestInput input)
    {
        if (!string.IsNullOrEmpty(input.TestId))
        {
            var getActivatedTestDefinitionQuery = new Template<TestDefinition>(@"
                [
                    {
                        '$match': {
                            '_id': <id>,
                            'Status': <status>,
                            'TestAccessSettings.Settings._t': 'PublicLinkType'
                        }
                    }
                ]
            ").Tag("id", input.TestId).Tag("status", $"{((int)TestDefinitionStatus.Activated)}");

            var testDefinition = await DB.PipelineSingleAsync(getActivatedTestDefinitionQuery);

            return ToVerifyResult(testDefinition, Guid.NewGuid().ToString());
        }

        if (!string.IsNullOrEmpty(input.AccessCode))
        {
            var getActivatedTestDefinitionQuery = new Template<TestDefinition>(@"
                [
                    {
                        '$match': {
                            'Status': <status>,
                            'TestAccessSettings.Settings._t': 'PrivateAccessCodeType', 
                            'TestAccessSettings.Settings.Configs.Code': '<access_code>'
                        }
                    }
                ]
            ").Tag("status", $"{((int)TestDefinitionStatus.Activated)}").Tag("access_code", input.AccessCode);

            var testDefinition = await DB.PipelineSingleAsync(getActivatedTestDefinitionQuery);

            return ToVerifyResult(testDefinition, input.AccessCode);
        }

        throw new TestPlatformException("InvalidInput");
    }

    private static VerifyTestResult ToVerifyResult(TestDefinition testDefinition, string accessCode)
    {
        if (testDefinition == null)
        {
            return VerifyTestResult.Invalid();
        }

        var result = VerifyTestResult.Valid((testDefinition.ID, accessCode));
        result.TestName = testDefinition.BasicSettings.Name;

        var testStartSettings = testDefinition.TestStartSettings;
        if (testStartSettings != null)
        {
            result.InstructionMessage = testStartSettings.Instruction;
            result.ConsentMessage = testStartSettings.Consent;
        }

        return result;
    }

    public async Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input)
    {
        var existingExam = await DB.Find<Exam>().Match(c => c.TestId == input.TestId && c.AccessCode == input.AccessCode).ExecuteFirstAsync();
        if (existingExam != null)
        {
            return existingExam.ID;
        }

        var exam = new Exam
        {
            TestId = input.TestId,
            AccessCode = input.AccessCode,
            ExamineeInfo = input.ExamineeInfo
        };

        await DB.InsertAsync(exam);

        return exam.ID;
    }

    public async Task<ExamContentOutput> GenerateExamContent(GenerateExamContentInput input)
    {
        // Ensure exam is processed by order
        var exam = await DB.Find<Exam>().MatchID(input.ExamId).ExecuteFirstAsync();
        if (exam == null)
        {
            throw new TestPlatformException("NotFoundExam");
        }

        if (exam.Questions != null)
        {
            return new() { Questions = exam.Questions.Select(ToViewModel).ToArray() };
        }

        // Get test defintion and validate it
        var testDefinition = await DB.Find<TestDefinition>().MatchID(input.TestDefinitionId).ExecuteSingleAsync() ?? throw new TestPlatformException("NotFoundTestDefinition");

        // Get questions of test definition.
        var questions = await DB.Find<QuestionDefinition>().ManyAsync(c => c.TestId == testDefinition.ID);
        var testSet = testDefinition.GenerateTestSet(questions, exam.AccessCode);
        exam.Questions = testSet;
        await DB.SaveOnlyAsync(exam, new[] { nameof(Exam.Questions) });

        return new ExamContentOutput
        {
            Questions = testSet.Select(ToViewModel).ToArray()
        };
    }

    public async Task<FinishExamOutputViewModel> FinishExam(FinishExamInput input)
    {
        var exam = await DB.Find<Exam>().MatchID(input.ExamId).ExecuteFirstAsync();
        if (exam == null)
        {
            throw new TestPlatformException("NotFoundExam");
        }

        // Foreach questions, get point
        var answers = input.Answers;
        var totalPoints = 0;
        foreach (var question in exam.Questions)
        {
            string[]? answer;

            answers.TryGetValue(question.ID, out answer);
            totalPoints += question.CalculatePoint(answer);
        }

        //todo: store result to db. combine with grading settings.
        //todo: update exam status.
        return new FinishExamOutputViewModel
        {
            TotalPoints = totalPoints
        };
    }

    private static ExamQuestion ToViewModel(QuestionDefinition c)
    {
        return new ExamQuestion
        {
            Id = c.ID,
            Description = c.Description,
            AnswerType = c.AnswerType,
            Answers = c.Answers.Select(c => new ExamAnswer
            {
                Id = c.Id,
                Description = c.AnswerDescription,
            }).ToArray()
        };
    }
}
