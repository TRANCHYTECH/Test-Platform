using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestDefinition/{testId}/Question")]
[Authorize]
public class QuestionDefinitionManagementController : ControllerBase
{
    private readonly IQuestionManagerService _questionManagerService;

    public QuestionDefinitionManagementController(IQuestionManagerService questionManagerService)
    {
        _questionManagerService = questionManagerService;
    }

    [HttpPost]
    [ProducesResponseType(typeof(QuestionViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuestionViewModel>> Create(string testId, [FromBody] CreateOrUpdateQuestionViewModel viewModel, CancellationToken cancellationToken)
    {
        var question = await _questionManagerService.CreateQuestion(testId, viewModel, cancellationToken);

        return question;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedSearchResult<QuestionViewModel>), StatusCodes.Status200OK)]
    public async Task<PagedSearchResult<QuestionViewModel>> Get(string testId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken cancellationToken)
    {
        return await _questionManagerService.GetQuestions(testId, pageNumber ?? 1, pageSize ?? 10, cancellationToken);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuestionViewModel), StatusCodes.Status201Created)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var question = await _questionManagerService.GetQuestion(id, cancellationToken);

        if (question == null)
        {
            return NotFound();
        }

        return Ok(question);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="viewModel"></param>
    /// <returns></returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(QuestionViewModel), 200)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(string id, CreateOrUpdateQuestionViewModel viewModel, CancellationToken cancellationToken)
    {
        var updatedQuestion = await _questionManagerService.UpdateQuestion(id, viewModel, cancellationToken);

        return Ok(updatedQuestion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await _questionManagerService.DeleteQuestion(id, cancellationToken);

        return Ok();
    }

    [HttpGet("Summary")]
    [ProducesResponseType(typeof(IEnumerable<QuestionSummaryViewModel>), StatusCodes.Status200OK)]
    public Task<IEnumerable<QuestionSummaryViewModel>> GetQuestionSummary(string testId, CancellationToken cancellationToken)
    {
        return _questionManagerService.GetQuestionSummary(testId, cancellationToken);
    }
}