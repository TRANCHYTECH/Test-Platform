using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestManager.Api.Controllers
{
    [ApiController]
    [Route("Management/TestDefinition")]
    [Authorize]
    public class TestDefinitionManagementController(
        ILogger<TestDefinitionManagementController> logger,
        ITestManagerService testManagerService)
        : ControllerBase
    {
        private readonly ILogger<TestDefinitionManagementController> _logger = logger;

        [HttpPost]
        public async Task<IActionResult> Create(NewTestDefinitionViewModel viewModel)
        {
            var createdTest = await testManagerService.CreateTestDefinition(viewModel);

            return Ok(createdTest);
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] int? pageNumber, [FromQuery] int? pageSize,
            CancellationToken cancellationToken)
        {
            //todo: move paging search stuff to shared, also prevent max is 100
            var testDefinitions =
                await testManagerService.GetTestDefinitionOverviews(pageNumber ?? 1, pageSize ?? 12, cancellationToken);

            return Ok(testDefinitions);
        }

        /// <summary>
        /// </summary>
        /// <param name="id"></param>
        /// <param name="viewModel"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, UpdateTestDefinitionViewModel viewModel)
        {
            var testDefinitions = await testManagerService.UpdateTestDefinition(id, viewModel);

            return Ok(testDefinitions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var testDefinition = await testManagerService.GetTestDefinition(id);

            if (testDefinition == null)
            {
                return NotFound();
            }

            return Ok(testDefinition);
        }

        [HttpPost("{id}/Activate")]
        public async Task<IActionResult> Activate(string id, CancellationToken cancellationToken)
        {
            var testDefinition = await testManagerService.ActivateTestDefinition(id, cancellationToken);

            return Ok(testDefinition);
        }

        [HttpPost("{id}/End")]
        public async Task<IActionResult> End(string id, CancellationToken cancellationToken)
        {
            var testDefinition = await testManagerService.EndTestDefinition(id, cancellationToken);

            return Ok(testDefinition);
        }

        [HttpPost("{id}/Restart")]
        public async Task<IActionResult> Restart(string id)
        {
            var testDefinition = await testManagerService.RestartTestDefinition(id);

            return Ok(testDefinition);
        }

        [HttpGet("{id}/TestAccess/GenerateAccessCodes/{quantity:range(1,50)}")]
        public async Task<IActionResult> GenerateAccessCodes(string id, int quantity)
        {
            var result = await testManagerService.GenerateAccessCodes(id, quantity);

            return Ok(result);
        }

        [HttpPost("{id}/TestAccess/SendAccessCodes")]
        public async Task<IActionResult> SendAccessCodes(string id, [FromBody] string[] codes)
        {
            await testManagerService.SendAccessCodes(id, codes);

            return Ok();
        }

        [HttpDelete("{id}/TestAccess/RemoveAccessCodes")]
        public async Task<IActionResult> RemoveAccessCode(string id, [FromQuery(Name = "code")] string[] codes)
        {
            var result = await testManagerService.RemoveAccessCodes(id, codes);

            return Ok(result);
        }

        [HttpPost("{id}/TestInvitationStats")]
        public async Task<IActionResult> GetTestInvitationEvents(string id, TestInvitationStatsViewModel model)
        {
            // var result = await testManagerService.GetTestInvitationEvents(new TestInvitationStatsInput
            // {
            //     TestDefinitionId = id,
            //     TestRunId = model.TestRunId,
            //     AccessCodes = model.AccessCodes
            // });

            return Ok(default);
        }
    }
}