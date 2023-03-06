using Dapr.Actors.Client;
using Dapr.Actors;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;

namespace VietGeeks.TestPlatform.TestRunner.Api.Controllers;
[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    [HttpGet("Start/{code}")]
    public async Task<IActionResult> StartTest(string code)
    {
        var actorId = new ActorId(code);
        var proxy = ActorProxy.Create<IProctorActor>(actorId, "ProctorActor");
        var startedTest = await proxy.StartTest(code);

        return Ok(startedTest);
    }
}
