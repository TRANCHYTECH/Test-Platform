using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Infrastructure;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;

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
        var testDefinitions = await _testManagerService.GetTestDefinitions();

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

        //todo(tau): remove this mock.
        viewModel.GradingSettings = new CreateOrupdateGradingSettings
        {
            TestEndConfig = new TestEndConfig
            {
                Message = "Custom message",
                RedirectTo = true,
                ToAddress = "https://vietgeeks.io"
            },
            GradingCriterias = new Dictionary<string, GradingCriteriaConfig>
            {
                {
                    GradingCriteriaConfigType.PassMask.ToString(),
                    new PassMaskCriteria {
                        Value = 70,
                        Unit = RangeUnit.Point
                    }
                },
                {
                    GradingCriteriaConfigType.GradeRanges.ToString(),
                    new GradeRangeCriteria {
                        Unit = RangeUnit.Percent,
                        GradeType = GradeType.GradeAndDescriptive,
                        Details = new List<GradeRangeCriteriaDetail>
                        {
                            new GradeRangeCriteriaDetail
                            {
                                From = 0,
                                To = 50,
                                Grades = new Dictionary<string, string>{
                                    { GradeType.Grade.ToString(), "Grade A" },
                                    { GradeType.Descriptive.ToString(), "Superman" }
                                }
                            }
                        }
                    }
                }
            }
        };

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
}