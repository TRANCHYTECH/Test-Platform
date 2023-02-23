using FluentValidation;
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
    public async Task<ActionResult<QuestionViewModel>> Create([FromServices] IValidator<NewQuestionViewModel> newQuestionValidator, string testId, [FromBody] NewQuestionViewModel viewModel, CancellationToken cancellationToken)
    {
        var validationResult = newQuestionValidator.Validate(viewModel);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var question = await _questionManagerService.CreateQuestion(testId, viewModel, cancellationToken);

        return question;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<QuestionViewModel>), StatusCodes.Status201Created)]
    //TODO: paging
    public Task<IEnumerable<QuestionViewModel>> Get(string testId, int? pageIndex, CancellationToken cancellationToken)
    {
        return _questionManagerService.GetQuestions(testId, cancellationToken);
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
    public async Task<IActionResult> Update([FromServices] IValidator<QuestionViewModel> questionValidator, string id, QuestionViewModel viewModel, CancellationToken cancellationToken)
    {
        var validationResult = questionValidator.Validate(viewModel);
        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

        var updatedQuestion = await _questionManagerService.UpdateQuestion(id, viewModel, cancellationToken);

        return Ok(updatedQuestion);
    }

    [HttpGet("Summary")]
    [ProducesResponseType(typeof(IEnumerable<QuestionSummaryViewModel>), StatusCodes.Status200OK)]
    public Task<IEnumerable<QuestionSummaryViewModel>> GetQuestionSummary(string testId, CancellationToken cancellationToken)
    {
        return _questionManagerService.GetQuestionSummary(testId, cancellationToken);
    }
}