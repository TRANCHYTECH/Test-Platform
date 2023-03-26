using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestDefinition")]
[Authorize]
public class TestDefinitionManagementController : ControllerBase
{
    private readonly ILogger<TestDefinitionManagementController> _logger;
    private readonly IValidator<UpdateTestDefinitionViewModel> _updateTestValidator;
    private readonly ITestManagerService _testManagerService;

    public TestDefinitionManagementController(ILogger<TestDefinitionManagementController> logger,
        IValidator<NewTestDefinitionViewModel> newTestValidator,
        IValidator<UpdateTestDefinitionViewModel> updateTestValidator,
        ITestManagerService testManagerService)
    {
        _logger = logger;
        _updateTestValidator = updateTestValidator;
        _testManagerService = testManagerService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromServices] IValidator<NewTestDefinitionViewModel> newTestValidator, NewTestDefinitionViewModel viewModel)
    {
        var validationResult = newTestValidator.Validate(viewModel);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var createdTest = await _testManagerService.CreateTestDefinition(viewModel);

        return Ok(createdTest);
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        List<TestDefinitionOverview> testDefinitions = await _testManagerService.GetTestDefinitionOverviews();

        return Ok(testDefinitions);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, UpdateTestDefinitionViewModel viewModel)
    {
        var validationResult = _updateTestValidator.Validate(viewModel);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var testDefinitions = await _testManagerService.UpdateTestDefinition(id, viewModel);

        return Ok(testDefinitions);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> Get(string id)
    {
        var testDefinition = await _testManagerService.GetTestDefinition(id);

        if (testDefinition == null)
        {
            return NotFound();
        }

        return Ok(testDefinition);
    }

    [HttpPost("{id}/Activate")]
    public async Task<IActionResult> Activate(string id)
    {
        var testDefiniton = await _testManagerService.ActivateTestDefinition(id);

        return Ok(testDefiniton);
    }

    [HttpPost("{id}/End")]
    public async Task<IActionResult> End(string id)
    {
        var testDefiniton = await _testManagerService.EndTestDefinition(id);

        return Ok(testDefiniton);
    }

    [HttpPost("{id}/Restart")]
    public async Task<IActionResult> Restart(string id)
    {
        TestDefinitionViewModel testDefiniton = await _testManagerService.RestartTestDefinition(id);

        return Ok(testDefiniton);
    }
}