using Dapr.Actors.Client;
using Dapr.Actors;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;
using VietGeeks.TestPlatform.TestRunner.Contract;
using System.Text;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestRunner.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class TestController : ControllerBase
{
    private readonly IProctorService _proctorService;

    public TestController(IProctorService proctorService)
    {
        _proctorService = proctorService;
    }

    [HttpPost("PreStart/Verify")]
    public async Task<IActionResult> Verify(VerifyTestInput input)
    {
        var verifyResult = await _proctorService.VerifyTest(input);
        if (!verifyResult.IsValid)
        {
            return BadRequest();
        }

        SetTestSession(new()
        {
            TestId = verifyResult.TestId,
            AccessCode = verifyResult.AccessCode,
            PreviousStep = PreStartSteps.Verified,
            ClientProof = "some data"
        });

        return Ok();
    }

    [HttpPost("PreStart/ProvideExamineeInfo")]
    public async Task<IActionResult> ProvideExamineeInfo(ProvideExamineeInfoViewModel data)
    {
        var testSession = GetTestSession();
        if (testSession.PreviousStep != PreStartSteps.Verified)
        {
            return BadRequest("Invalid Step");
        }

        var examId = await _proctorService.ProvideExamineeInfo(new()
        {
            TestId = testSession.TestId,
            AccessCode = testSession.AccessCode,
            ExamineeInfo = data.ExamineeInfo
        });

        SetTestSession(new()
        {
            ExamId = examId,
            TestId = testSession.TestId,
            AccessCode = testSession.AccessCode,
            PreviousStep = PreStartSteps.ProvidedExamineeInfo,
            ClientProof = testSession.ClientProof
        });

        return Ok();
    }

    [HttpPost("Start")]
    public async Task<IActionResult> StartTest()
    {
        var testSession = GetTestSession();
        if (testSession.PreviousStep != PreStartSteps.ProvidedExamineeInfo)
        {
            return BadRequest("Invalid Step");
        }

        // Generate exam content: questions.
        var examContent = await _proctorService.GenerateExamContent(new()
        {
            TestDefinitionId = testSession.TestId,
            AccessCode = testSession.AccessCode,
        });

        var proctorExamActor = GetProctorActor(testSession);
        await proctorExamActor.StartExam(new()
        {
            ExamId = testSession.ExamId,
            TestDefinitionId = testSession.TestId,
            Questions = examContent.Questions.ToDictionary(c => c.Id, d => d.Answers.Select(a => a.Id).ToArray())
        });

        SetTestSession(new()
        {
            ExamId = testSession.ExamId,
            TestId = testSession.TestId,
            AccessCode = testSession.AccessCode,
            PreviousStep = PreStartSteps.Started,
            ClientProof = testSession.ClientProof
        });

        return Ok(examContent);
    }

    [HttpPost("SubmitAnwser")]
    public async Task<IActionResult> SubmitAnswer(SubmitAnswerViewModel data)
    {
        var testSession = GetTestSession();
        if (testSession.PreviousStep != PreStartSteps.Started)
        {
            return BadRequest("Invalid Step");
        }

        var proctorExamActor = GetProctorActor(testSession);
        await proctorExamActor.SubmitAnswer(new()
        {
            QuestionId = data.QuestionId,
            AnswerId = data.AnswerId
        });

        return Ok();
    }

    private static IProctorActor GetProctorActor(TestSession testSession)
    {
        return ActorProxy.Create<IProctorActor>(new ActorId(testSession.ExamId), "ProctorActor");
    }

    private void SetTestSession(TestSession testSession)
    {
        Response.Headers.Add(nameof(TestSession), EncryptTestSession(testSession));
    }

    private TestSession GetTestSession()
    {
        var session = Request.Headers[nameof(TestSession)];

        return DecryptTestSession(session.ToString());
    }

    private static string EncryptTestSession(TestSession session)
    {
        //todo: Use data protection feature of .net
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(System.Text.Json.JsonSerializer.Serialize(session)));
    }

    private static TestSession DecryptTestSession(string token)
    {
        var str = Encoding.ASCII.GetString(Convert.FromBase64String(token)) ?? throw new InvalidCastException();
        var session = System.Text.Json.JsonSerializer.Deserialize<TestSession>(str);
        return session ?? throw new InvalidCastException();
    }
}

