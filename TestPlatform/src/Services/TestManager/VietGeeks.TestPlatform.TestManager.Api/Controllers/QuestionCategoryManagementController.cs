using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/QuestionCategory")]
[Authorize]
public class QuestionCategoryManagementController : ControllerBase
{
    private readonly ILogger<QuestionCategoryManagementController> _logger;
    private readonly IQuestionCategoryService _questionCategoryService;

    public QuestionCategoryManagementController(ILogger<QuestionCategoryManagementController> logger, IQuestionCategoryService questionCategoryService)
    {
        _logger = logger;
        _questionCategoryService = questionCategoryService;
    }

    [HttpGet]
    public async Task<IActionResult> Get(CancellationToken cancellationToken)
    {
        var testCategories = await _questionCategoryService.GetCategories(cancellationToken);

        return Ok(testCategories);
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestionCategoryViewModel), 200)]
    public async Task<IActionResult> Create(NewQuestionCategoryViewModel viewModel, CancellationToken cancellationToken)
    {
        var questionCategory = await _questionCategoryService.CreateQuestionCategory(viewModel, cancellationToken);

        return Ok(questionCategory);
    }
}

