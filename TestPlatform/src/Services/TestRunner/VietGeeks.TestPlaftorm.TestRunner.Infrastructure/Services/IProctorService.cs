using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;
public interface IProctorService
{
    Task<VerifyTestResultViewModel> VerifyTest(VerifyTestInput viewModel);

    Task<string> ProvideExamineeInfo(ProvideExamineeInfoInput viewModel);

    Task<ExamContentOutput> GenerateExamContent(GenerateExamContentInput input);
}