using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestReport")]
[Authorize]
public class TestReportController(ITestReportService testReportService) : ControllerBase
{
    [HttpGet("{testId}/TestRuns")]
    public async Task<IActionResult> GetTestRuns(string testId)
    {
        var testRuns = await testReportService.GetTestRunSummaries(testId);

        return Ok(testRuns);
    }

    [HttpGet("Exams")]
    public async Task<IActionResult> ListExams([FromQuery] string[] testRunIds)
    {
        var summaries = await testReportService.GetExamSummaries(testRunIds);

        return Ok(summaries);
    }

    [HttpGet("Respondents")]
    public async Task<IActionResult> ListRespondents([FromQuery] string[] testRunIds)
    {
        var respondents = await testReportService.GetRespondents(testRunIds);

        return Ok(respondents);
    }

    [HttpGet("Exam/{examId}/Review")]
    public async Task<IActionResult> GetExamReview(string examId)
    {
        var result = await testReportService.GetExamReview(examId);

        return Ok(result);
    }
}