using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestCategory")]
[Authorize]
public class TestCategoryManagementController : ControllerBase
{
    private readonly ILogger<TestCategoryManagementController> _logger;
    private readonly ITestManagerService _testManagerService;

    public TestCategoryManagementController(ILogger<TestCategoryManagementController> logger, ITestManagerService testManagerService)
    {
        _logger = logger;
        _testManagerService = testManagerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var testCategories = await _testManagerService.GetTestCategories();

        return Ok(testCategories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewTestCategoryViewModel viewModel)
    {
        //todo(tau): add validator
        var createdTestCategory = await _testManagerService.CreateTestCategory(viewModel);

        return Ok(createdTestCategory);
    }
}

