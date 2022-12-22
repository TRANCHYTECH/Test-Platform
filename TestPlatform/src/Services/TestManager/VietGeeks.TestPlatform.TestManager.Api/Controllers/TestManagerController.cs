using FluentValidation;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Api.ValidationRules;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize]
    public class TestManagerController : ControllerBase
    {
        private readonly ILogger<TestManagerController> _logger;

        private readonly IValidator<NewTestViewModel> _newTestValidator;
        private readonly ITestManagerService _testManagerService;

        public TestManagerController(ILogger<TestManagerController> logger, IValidator<NewTestViewModel> newTestValidator, ITestManagerService testManagerService)
        {
            _logger = logger;
            _newTestValidator = newTestValidator;
            _testManagerService = testManagerService;
        }

        [HttpPost(Name = "Create")]
        public async Task<IActionResult> CreateAsync(NewTestViewModel viewModel)
        {
            var validationResult = _newTestValidator.Validate(viewModel);
            if(!validationResult.IsValid)
            {
                validationResult.AddToModelState(this.ModelState);

                return BadRequest(ModelState);
            }

            var createdTest = await _testManagerService.CreateTest(viewModel);

            return Ok(createdTest);
        }
    }
}