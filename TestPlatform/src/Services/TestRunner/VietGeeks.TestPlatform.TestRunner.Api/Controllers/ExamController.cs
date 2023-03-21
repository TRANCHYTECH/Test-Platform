using Dapr.Actors.Client;
using Dapr.Actors;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.TestRunner.Contract;
using System.Text;
using VietGeeks.TestPlaftorm.TestRunner.Infrastructure.Services;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using System.Text.Json;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;
using System.Linq;
using System.Net;
using Swashbuckle.AspNetCore.Annotations;

namespace VietGeeks.TestPlatform.TestRunner.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExamController : ControllerBase
{
    private readonly IProctorService _proctorService;

    public ExamController(IProctorService proctorService)
    {
        _proctorService = proctorService;
    }

    [HttpPost("PreStart/Verify")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VerifyTestOutputViewModel))]
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
            PreviousStep = PreStartSteps.VerifyTest,
            ClientProof = "some data"
        });

        return Ok(new VerifyTestOutputViewModel
        {
            TestName = verifyResult.TestName,
            ConsentMessage = verifyResult.ConsentMessage ?? "DefaultConsentMessage",
            InstructionMessage = verifyResult.InstructionMessage ?? "DefaultInstructionMessage"
        });
    }

    [HttpPost("PreStart/ProvideExamineeInfo")]
    public async Task<IActionResult> ProvideExamineeInfo(ProvideExamineeInfoViewModel data)
    {
        var testSession = GetTestSession(PreStartSteps.ProvideExamineeInfo);
        //todo: move this logic to Exam actor to prevent frault if mutiple submiting from browsers.
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
            PreviousStep = PreStartSteps.ProvideExamineeInfo,
            ClientProof = testSession.ClientProof
        });

        return Ok();
    }

    [HttpPost("Start")]
    [ProducesResponseType(typeof(StartExamOutput), 200)]
    public async Task<IActionResult> StartExam()
    {
        var testSession = GetTestSession(PreStartSteps.Start);
        var proctorExamActor = GetProctorActor(testSession);
        var examContent = await proctorExamActor.StartExam(new()
        {
            ExamId = testSession.ExamId,
            TestDefinitionId = testSession.TestId
        });

        SetTestSession(new()
        {
            ExamId = testSession.ExamId,
            TestId = testSession.TestId,
            AccessCode = testSession.AccessCode,
            PreviousStep = PreStartSteps.Start,
            ClientProof = testSession.ClientProof
        });

        return Ok(examContent);
    }

    [HttpPost("SubmitAnswer")]
    public async Task<IActionResult> SubmitAnswer(SubmitAnswerViewModel data)
    {
        var testSession = GetTestSession(PreStartSteps.SubmitAnswer);
        var proctorExamActor = GetProctorActor(testSession);
        await proctorExamActor.SubmitAnswer(new()
        {
            QuestionId = data.QuestionId,
            AnswerIds = data.AnswerIds
        });

        return Ok();
    }

    [HttpPost("Finish")]
    public async Task<IActionResult> FinishExam()
    {
        var testSession = GetTestSession(PreStartSteps.FinishExam);
        var proctorExamActor = GetProctorActor(testSession);
        var result = await proctorExamActor.FinishExam();

        return Ok(result);
    }

    private static IProctorActor GetProctorActor(TestSession testSession)
    {
        return ActorProxy.Create<IProctorActor>(new ActorId(testSession.ExamId), "ProctorActor");
    }

    private void SetTestSession(TestSession testSession)
    {
        Response.Headers.Add("Access-Control-Expose-Headers", nameof(TestSession));
        Response.Headers.Add(nameof(TestSession), EncryptTestSession(testSession));
    }

    private TestSession GetTestSession(PreStartSteps forStep)
    {
        var header = Request.Headers[nameof(TestSession)];
        var session = DecryptTestSession(header.ToString());

        switch (forStep)
        {
            case PreStartSteps.ProvideExamineeInfo:
            {
                if(session.PreviousStep != PreStartSteps.VerifyTest) {
                    throw new TestPlatformException("InvalidStep");
                }
                break;
            }
            
            case PreStartSteps.Start:
            {
                if(session.PreviousStep != PreStartSteps.ProvideExamineeInfo) {
                    throw new TestPlatformException("InvalidStep");
                }
                break;
            }

            case PreStartSteps.SubmitAnswer:
            case PreStartSteps.FinishExam:
            {
                if(session.PreviousStep != PreStartSteps.Start) {
                    throw new TestPlatformException("InvalidStep");
                }
                break;
            }
        }

        return session;
    }

    private static string EncryptTestSession(TestSession session)
    {
        //todo: Use data protection feature of .net
        return Convert.ToBase64String(Encoding.ASCII.GetBytes(JsonSerializer.Serialize(session)));
    }

    private static TestSession DecryptTestSession(string token)
    {
        try
        {
            var str = Encoding.ASCII.GetString(Convert.FromBase64String(token));
            return JsonSerializer.Deserialize<TestSession>(str) ?? throw new TestPlatformException("InvalidTestSession");
        }
        catch (Exception ex)
        {
            throw new TestPlatformException("InvalidTestSession", ex);
        }

    }
}

