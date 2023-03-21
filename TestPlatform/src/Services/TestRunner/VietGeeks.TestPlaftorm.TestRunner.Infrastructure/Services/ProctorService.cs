using AutoMapper;
using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core.Logics;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

public class ProctorService : IProctorService
{
    private readonly IMapper _mapper;

    public ProctorService(IMapper mapper) {
        _mapper = mapper;
    }
    
    //todo: PLAN - need to refactor this class, instead of directly access to original database, it should be a separate microservice.
    public async Task<VerifyTestOutput> VerifyTest(VerifyTestInput input)
    {
        if (!string.IsNullOrEmpty(input.TestId))
        {
            var getActivatedTestQuery = new Template<TestDefinition>(@"
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

            var activatedTest = await DB.PipelineSingleAsync(getActivatedTestQuery);

            return activatedTest.ToOutput(Guid.NewGuid().ToString());
        }

        if (!string.IsNullOrEmpty(input.AccessCode))
        {
            var getActivatedTestQuery = new Template<TestDefinition>(@"
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

            var activatedTest = await DB.PipelineSingleAsync(getActivatedTestQuery);

            return activatedTest.ToOutput(input.AccessCode);
        }

        throw new TestPlatformException("InvalidInput");
    }

    public async Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input)
    {
        var exam = await GetExam(input.TestId, input.AccessCode);
        if (exam == null)
        {
            exam = new Exam
            {
                TestId = input.TestId,
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
        if (exam.Questions == null)
        {
            var testDefinition = await GetTestDefinition(input.TestDefinitionId);
            var timeSettings = testDefinition.TimeSettings ?? throw new Exception("TimeSettings not configured properly");
            var gradingSettings = testDefinition.GradingSettings ?? throw new Exception("GradingSettings not configured properly");

            // Get questions of test definition.
            var testQuestions = await GetTestQuestions(testDefinition.ID);

            exam.Questions = testDefinition.GenerateTestSet(testQuestions, exam.AccessCode);
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
        exam.Grading = exam.GradeSettings.CalculateGrading(exam.FinalMark, exam.Questions.Sum(c=>c.ScoreSettings.TotalPoints));
        // Calculate pass grading.
        await DB.SaveOnlyAsync(exam, new[] { nameof(Exam.StartedAt), nameof(Exam.FinishedAt), nameof(Exam.FinalMark) });

        return new FinishExamOutput
        {
            FinalMark = exam.FinalMark,
            Grading = _mapper.Map<List<AggregatedGrading>>(exam.Grading)
        };
    }

    private static async Task<Exam> GetExam(string testId, string accessCode)
    {
        return await DB.Find<Exam>().Match(c => c.TestId == testId && c.AccessCode == accessCode).ExecuteFirstAsync();
    }

    private static async Task<Exam> GetExam(string examId)
    {
        return await DB.Find<Exam>().MatchID(examId).ExecuteSingleAsync();
    }

    private static async Task<Exam> EnsureExam(string examId)
    {
        return await GetExam(examId) ?? throw new TestPlatformException("NotFoundExam");
    }

    private static async Task<List<QuestionDefinition>> GetTestQuestions(string testDefinitionId)
    {
        return await DB.Find<QuestionDefinition>().ManyAsync(c => c.TestId == testDefinitionId);
    }

    private static async Task<TestDefinition> GetTestDefinition(string testDefinitionId)
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
