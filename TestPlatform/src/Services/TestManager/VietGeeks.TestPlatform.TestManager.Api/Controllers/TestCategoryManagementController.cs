using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestCategory")]
[Authorize]
public class TestCategoryManagementController : ControllerBase
{
    private readonly ILogger<TestCategoryManagementController> _logger;
    private readonly ITestCategoryService _testCatalogService;

    public TestCategoryManagementController(ILogger<TestCategoryManagementController> logger, ITestCategoryService testManagerService)
    {
        _logger = logger;
        _testCatalogService = testManagerService;
    }

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var testCategories = await _testCatalogService.GetTestCategories();

        return Ok(testCategories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewTestCategoryViewModel viewModel)
    {
        var createdTestCategory = await _testCatalogService.CreateTestCategory(viewModel);

        return Ok(createdTestCategory);
    }
}

