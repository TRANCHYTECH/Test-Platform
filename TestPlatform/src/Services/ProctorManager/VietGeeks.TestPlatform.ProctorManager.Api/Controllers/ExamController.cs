using Microsoft.AspNetCore.Mvc;

namespace VietGeeks.TestPlatform.ProctorManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExamController : ControllerBase
    {
        private readonly ILogger<ExamController> _logger;

        public ExamController(ILogger<ExamController> logger)
        {
            _logger = logger;
        }

        [HttpGet("Content")]
        public IActionResult GetExamContent()
        {
            return Ok(new ExamContentViewModel
            {
                ExamId = Guid.NewGuid().ToString()
            });
        }
    }

    public class ExamContentViewModel
    {
        public string ExamId { get; set; } = default!;
    }
}