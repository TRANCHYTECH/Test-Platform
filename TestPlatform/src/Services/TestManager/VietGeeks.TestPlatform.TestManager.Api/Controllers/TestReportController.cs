using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers
{
    [ApiController]
    [Route("Management/TestReport")]
    [Authorize]
    public class TestReportController : ControllerBase
    {
        private readonly ITestReportService _testReportService;

        public TestReportController(ITestReportService testReportService) {
            _testReportService = testReportService;
        }

        [HttpGet("{testId}/TestRuns")]
        public async Task<IActionResult> GetTestRuns(string testId) {
            var summaries = await _testReportService.GetTestRunSummaries(testId);

            return Ok(summaries);
        }

        [HttpGet("Exams")]
        public async Task<IActionResult> ListExams([FromQuery]string[] testRunIds) {
            var summaries = await _testReportService.GetExamSummaries(testRunIds);

            return Ok(summaries);
        }
    }
}