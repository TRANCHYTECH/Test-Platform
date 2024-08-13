using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers;

[ApiController]
[Route("Management/TestDefinition/{testId}/Question")]
[Authorize]
public class QuestionDefinitionManagementController(IQuestionManagerService questionManagerService) : ControllerBase
{
    [HttpPost]
    [ProducesResponseType(typeof(QuestionViewModel), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<QuestionViewModel>> Create(string testId, [FromBody] CreateOrUpdateQuestionViewModel viewModel, CancellationToken cancellationToken)
    {
        var question = await questionManagerService.CreateQuestion(testId, viewModel, cancellationToken);

        return question;
    }

    [HttpGet]
    [ProducesResponseType(typeof(PagedSearchResult<QuestionViewModel>), StatusCodes.Status200OK)]
    public async Task<PagedSearchResult<QuestionViewModel>> Get(string testId, [FromQuery] int? pageNumber, [FromQuery] int? pageSize, CancellationToken cancellationToken)
    {
        return await questionManagerService.GetQuestions(testId, pageNumber ?? 1, pageSize ?? 10, cancellationToken);
    }

    [HttpGet("Order")]
    public async Task<IActionResult> QuestionOrders(string testId, CancellationToken cancellation)
    {
        var questions = await questionManagerService.GetQuestions(testId, cancellation);
        return Ok(questions);
    }

    [HttpPut("Order")]
    public async Task<IActionResult> UpdateQuestionOrders(string testId, [FromBody]UpdateQuestionOrderViewModel[] viewModel,  CancellationToken cancellation)
    {
        //todo: impl
        await questionManagerService.UpdateQuestionOrders(testId, viewModel, cancellation);
        return Ok();
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(QuestionViewModel), StatusCodes.Status201Created)]
    public async Task<IActionResult> GetById(string id, CancellationToken cancellationToken)
    {
        var question = await questionManagerService.GetQuestion(id, cancellationToken);

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
        var updatedQuestion = await questionManagerService.UpdateQuestion(id, viewModel, cancellationToken);

        return Ok(updatedQuestion);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        await questionManagerService.DeleteQuestion(id, cancellationToken);

        return Ok();
    }

    [HttpGet("Summary")]
    [ProducesResponseType(typeof(IEnumerable<QuestionSummaryViewModel>), StatusCodes.Status200OK)]
    public Task<IEnumerable<QuestionSummaryViewModel>> GetQuestionSummary(string testId, CancellationToken cancellationToken)
    {
        return questionManagerService.GetQuestionSummary(testId, cancellationToken);
    }
}