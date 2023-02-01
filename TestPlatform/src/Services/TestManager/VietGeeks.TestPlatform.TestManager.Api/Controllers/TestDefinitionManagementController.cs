using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Api.ValidationRules;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestDefinition")]
[Authorize]
public class TestDefinitionManagementController : ControllerBase
{
    private readonly ILogger<TestDefinitionManagementController> _logger;

    private readonly IValidator<NewTestDefinitionViewModel> _newTestValidator;
    private readonly ITestManagerService _testManagerService;

    public TestDefinitionManagementController(ILogger<TestDefinitionManagementController> logger, IValidator<NewTestDefinitionViewModel> newTestValidator, ITestManagerService testManagerService)
    {
        _logger = logger;
        _newTestValidator = newTestValidator;
        _testManagerService = testManagerService;
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewTestDefinitionViewModel viewModel)
    {
        var validationResult = _newTestValidator.Validate(viewModel);
        if(!validationResult.IsValid)
        {
            validationResult.AddToModelState(this.ModelState);

            return BadRequest(ModelState);
        }

        var createdTest = await _testManagerService.CreateTestDefinition(viewModel);

        return Ok(createdTest);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var testDefinitions = await _testManagerService.GetTestDefinitions();

        return Ok(testDefinitions);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateTestDefinitionViewModel viewModel)
    {
        var testDefinitions = await _testManagerService.UpdateTestDefinition(id, viewModel);

        return Ok(testDefinitions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var testDefinition = await _testManagerService.GetTestDefinition(id);

        return Ok(testDefinition);
    }
}