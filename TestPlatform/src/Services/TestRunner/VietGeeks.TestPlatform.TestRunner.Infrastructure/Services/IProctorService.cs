using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;
public interface IProctorService
{
    Task<VerifyTestOutput> VerifyTest(VerifyTestInput input);

    Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input);

    Task<ExamContentOutput> GenerateExamContent(GenerateExamContentInput input);

    Task<FinishExamOutput> FinishExam(FinishExamInput input);
    Task<ExamQuestion?> GetTestRunQuestion(string examId, string questionId);
}