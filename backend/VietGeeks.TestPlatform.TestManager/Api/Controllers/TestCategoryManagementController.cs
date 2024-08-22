using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestCategory")]
[Authorize]
public class TestCategoryManagementController(
    ILogger<TestCategoryManagementController> logger,
    ITestCategoryService testManagerService)
    : ControllerBase
{
    private readonly ILogger<TestCategoryManagementController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get()
    {
        var testCategories = await testManagerService.GetTestCategories();

        return Ok(testCategories);
    }

    [HttpPost]
    public async Task<IActionResult> Create(NewTestCategoryViewModel viewModel)
    {
        var createdTestCategory = await testManagerService.CreateTestCategory(viewModel);

        return Ok(createdTestCategory);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string[] ids)
    {
        await testManagerService.DeleteTestCategories(ids);

        return Ok();
    }
}