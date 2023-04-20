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
    public async Task<IActionResult> Create(NewTestDefinitionViewModel viewModel)
    {
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

    [HttpGet("{id}/TestAccess/GenerateAccessCodes/{quantity:range(1,50)}")]
    public async Task<IActionResult> GenerateAccessCodes(string id, int quantity)
    {
        List<string> accessCodes = await _testManagerService.GenerateAccessCodes(id, quantity);

        return Ok(new { AccessCodes = accessCodes });
    }

    [HttpDelete("{id}/TestAccess/RemoveAccessCode/{code}")]
    public async Task<IActionResult> RemoveAccessCode(string id, string code)
    {
        await _testManagerService.RemoveAccessCode(id, code);

        return Ok();
    }

    [HttpPost("{id}/TestInvitationStats")]
    public async Task<IActionResult> GetTestInvitationEvents(string id, TestInvitationStatsViewModel model)
    {
        var result = await _testManagerService.GetTestInvitationEvents(new()
        {
            TestDefinitionId = id,
            TestRunId = model.TestRunId,
            AccessCodes = model.AccessCodes
        });

        return Ok(result);
    }
}

