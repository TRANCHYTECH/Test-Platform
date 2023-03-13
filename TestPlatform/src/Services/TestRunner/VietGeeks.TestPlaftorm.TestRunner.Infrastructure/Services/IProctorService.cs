using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;
public interface IProctorService
{
    Task<VerifyTestResultViewModel> VerifyTest(VerifyTestViewModel viewModel);

    Task<string> ProvideExamineeInfo(ProvideExamineeInfoInputViewModel viewModel);

    Task<List<dynamic>> GenerateExamContent(GenerateExamContentInput input);
}