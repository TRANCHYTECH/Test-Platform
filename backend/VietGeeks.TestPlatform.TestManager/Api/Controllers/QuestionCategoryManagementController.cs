using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

//todo: take into account the test id because the test if already activated, question or categories, not allow to be changed.
[ApiController]
[Route("Management/TestDefinition/{testId}/QuestionCategory")]
[Authorize]
public class QuestionCategoryManagementController(
    ILogger<QuestionCategoryManagementController> logger,
    IQuestionCategoryService questionCategoryService)
    : ControllerBase
{
    private readonly ILogger<QuestionCategoryManagementController> _logger = logger;

    [HttpGet]
    public async Task<IActionResult> Get(string testId, CancellationToken cancellationToken)
    {
        var categories = await questionCategoryService.GetCategories(testId, cancellationToken);

        return Ok(categories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestionCategoryViewModel), 200)]
    public async Task<IActionResult> Create(string testId, NewQuestionCategoryViewModel viewModel,
        CancellationToken cancellationToken)
    {
        var createdCategory = await questionCategoryService.CreateCategory(testId, viewModel, cancellationToken);

        return Ok(createdCategory);
    }

    [HttpDelete]
    public async Task<IActionResult> Delete([FromQuery] string[] ids)
    {
        await questionCategoryService.DeleteCategories(ids);

        return Ok();
    }
}