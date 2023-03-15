using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core.Logics;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

public class ProctorService : IProctorService
{
    public async Task<VerifyTestResultViewModel> VerifyTest(VerifyTestInput input)
    {
        if (!string.IsNullOrEmpty(input.TestId))
        {
            var testDefinition = await DB.Find<TestDefinition>().Match(c => c.ID == input.TestId && c.Status == TestDefinitionStatus.Activated).ExecuteFirstAsync();

            return ToVerifyResult(testDefinition);
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

    private static VerifyTestResultViewModel ToVerifyResult(TestDefinition testDefinition, string? accessCode = null)
    {
        if (testDefinition == null)
        {
            return VerifyTestResultViewModel.Invalid();
        }

        var res = VerifyTestResultViewModel.Valid((testDefinition.ID, accessCode));
        var testStartSettings = testDefinition.TestStartSettings;
        if (testStartSettings != null)
        {
            res.InstructionMessage = testStartSettings.Instruction;
            res.ConsentMessage = testStartSettings.Consent;
        }

        return res;
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
        // Get test defintion and validate it
        var testDefinition = await DB.Find<TestDefinition>().MatchID(input.TestDefinitionId).ExecuteSingleAsync() ?? throw new TestPlatformException ("NotFoundTestDefinition");

        // Get questions of test definition.
        var questions = await DB.Find<QuestionDefinition>().ManyAsync(c => c.TestId == testDefinition.ID);
        var result = new ExamContentOutput
        {
            Questions = testDefinition.GenerateTestSet(questions, input.AccessCode).Select(ToViewModel).ToArray()
        };

        return result;
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
                Id = Guid.NewGuid().ToString(), //c.Id,
                Description = c.AnswerDescription
            }).ToArray()
        };
    }
}
