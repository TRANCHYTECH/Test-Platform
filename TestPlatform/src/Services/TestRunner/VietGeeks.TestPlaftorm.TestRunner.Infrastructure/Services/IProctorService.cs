using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;
public interface IProctorService
{
    Task<VerifyTestResult> VerifyTest(VerifyTestInput input);

    Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput input);

    Task<ExamContentOutput> GenerateExamContent(GenerateExamContentInput input);

    Task<FinishExamOutputViewModel> FinishExam(FinishExamInput input);
}