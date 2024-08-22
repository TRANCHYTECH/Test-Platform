using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlatform.TestRunner.Infrastructure.Services
{
    public interface IProctorService
    {
        Task<VerifyTestOutput> VerifyTest(VerifyTestInput input);

        Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input);

        Task<ExamContentOutput> GenerateExamContent(GenerateExamContentInput input);

        Task<FinishExamOutput> FinishExam(FinishExamInput input);
        Task<TestRun> GetTestRun(string testRunId);
        Task<QuestionDefinition?> GetTestRunQuestion(string examId, string questionId);
        Task<IEnumerable<QuestionDefinition>> GetTestRunQuestionsByExamId(string examId);
        Task<AfterTestConfigOutput> GetAfterTestConfigAsync(string examId);
        bool IsCorrectAnswer(QuestionDefinition questionDefinition, string[] answers);
    }
}