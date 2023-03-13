using Dapr.Actors.Client;
using Dapr.Actors;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;
using VietGeeks.TestPlatform.TestRunner.Contract;
using Microsoft.AspNetCore.Authorization;
using System.Text;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestRunner.Api.Controllers;

[ApiController]
[Route("[controller]")]
[AllowAnonymous]
public class TestController : ControllerBase
{
    private readonly IProctorService _proctorService;

    public TestController(IProctorService proctorService)
    {
        _proctorService = proctorService;
    }

    [HttpPost("PreStart/Verify")]
    public async Task<IActionResult> Verify(VerifyTestViewModel data)
    {
        var verifyResult = await _proctorService.VerifyTest(data);

        var session = EncryptToken(new TestStartContinousToken
        {
            TestId = verifyResult.TestId,
            AccessCode = verifyResult.AccessCode,
            PreviousStep = PreStartSteps.Verify,
            ClientFootage = "some data"
        });
        return Ok(new { continousToken = session });
    }

    [HttpPost("PreStart/ProvideExamineeInfo")]
    public async Task<IActionResult> ProvideExamineeInfo([FromHeader(Name = "ContinousToken")] string continousToken, ProvideExamineeInfoViewModel data)
    {
        var previousToken = DecryptToken(continousToken);

        var input = new ProvideExamineeInfoInputViewModel
        {
            TestId = previousToken.TestId,
            AccessCode = previousToken.AccessCode,
            ExamineeInfo = data.ExamineeInfo
        };

        var examId = await _proctorService.ProvideExamineeInfo(input);
        var session = new TestStartContinousToken
        {
            ExamId = examId,
            TestId = previousToken.TestId,
            AccessCode = previousToken.AccessCode,
            PreviousStep = PreStartSteps.ProvideExamineeInfo,
            ClientFootage = previousToken.ClientFootage
        };

        var encryptedSession = EncryptToken(session);
        return Ok(new { continousToken = encryptedSession });
    }

    [HttpPost("Start")]
    public async Task<IActionResult> StartTest([FromHeader(Name = "ContinousToken")] string continousToken)
    {
        var previousToken = DecryptToken(continousToken);
        if (previousToken == null)
        {
            return BadRequest();
        }

        // Generate exam content: questions.

        var examContent = await _proctorService.GenerateExamContent(new GenerateExamContentInput
        {
            TestDefinitionId = previousToken.TestId,
            AccessCode = previousToken.AccessCode,
        });

        //var actorId = new ActorId(previousToken.AccessCode);
        //var proxy = ActorProxy.Create<IProctorActor>(actorId, "ProctorActor");
        //var startedTest = await proxy.StartTest(previousToken.AccessCode);

        return Ok(examContent);
    }

    [HttpPost("SubmitAnwser")]
    public IActionResult SubmitAnwser([FromHeader(Name = "ContinousToken")] string continousToken)
    {
        return Ok();
    }

    private static string EncryptToken(TestStartContinousToken session)
    {

        //todo: Use data protection feature of .net
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(session)));
    }

    private static TestStartContinousToken DecryptToken(string token)
    {
        var str = Encoding.ASCII.GetString(Convert.FromBase64String(token)) ?? throw new InvalidCastException();
        var session = System.Text.Json.JsonSerializer.Deserialize<TestStartContinousToken>(str);
        return session ?? throw new InvalidCastException();
    }
}

