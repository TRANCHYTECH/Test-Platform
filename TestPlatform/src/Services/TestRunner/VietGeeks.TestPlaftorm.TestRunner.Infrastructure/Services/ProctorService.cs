using Dapr.Client;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core.Logics;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

public class ProctorService : IProctorService
{
    private readonly DaprClient _daprClient;

    public ProctorService(DaprClient daprClient)
    {
        _daprClient = daprClient;
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
            return new()
            {
                TestDuration = exam.TimeSettings.TestDurationMethod.ToViewModel(),
                Questions = exam.Questions.Select(ExamMapper.ToViewModel).ToArray()
            };
        }

        // Get test defintion and validate it
        var testDefinition = await DB.Find<TestDefinition>().MatchID(input.TestDefinitionId).ExecuteSingleAsync() ?? throw new TestPlatformException("NotFoundTestDefinition");
        var timeSettings = testDefinition.TimeSettings ?? throw new Exception("Test not configured properly");

        // Get questions of test definition.
        var questions = await DB.Find<QuestionDefinition>().ManyAsync(c => c.TestId == testDefinition.ID);
        var testSet = testDefinition.GenerateTestSet(questions, exam.AccessCode);
        exam.Questions = testSet;
        exam.TimeSettings = timeSettings;

        await DB.SaveOnlyAsync(exam, new[] { nameof(Exam.Questions), nameof(Exam.TimeSettings) });

        return new()
        {
            TestDuration = timeSettings.TestDurationMethod.ToViewModel(),
            Questions = testSet.Select(ExamMapper.ToViewModel).ToArray()
        };
    }

    public async Task<FinishExamOutputViewModel> FinishExam(FinishExamInput input)
    {
        var exam = await DB.Find<Exam>().MatchID(input.ExamId).ExecuteFirstAsync();
        if (exam == null)
        {
            throw new TestPlatformException("NotFoundExam");
        }

        var totalPoints = CalculateExamPoint(exam, input.Answers);

        exam.StartedAt = input.StartedAt;
        exam.FinishedAt = input.FinishededAt;
        exam.FinalPoints = totalPoints;

        await DB.SaveOnlyAsync(exam, new[] { nameof(Exam.StartedAt), nameof(Exam.FinishedAt), nameof(Exam.FinalPoints) });
        return new FinishExamOutputViewModel
        {
            TotalPoints = totalPoints
        };
    }

    private static int CalculateExamPoint(Exam exam, Dictionary<string, string[]> answers)
    {
        int totalPoints = 0;
        foreach (var question in exam.Questions)
        {
            string[]? answer;
            answers.TryGetValue(question.ID, out answer);
            totalPoints += question.CalculatePoint(answer);
        }

        return totalPoints;
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
}
