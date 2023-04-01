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
using System.Net;
using Microsoft.AspNetCore.DataProtection;

namespace VietGeeks.TestPlatform.TestRunner.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExamController : ControllerBase
{
    private readonly IProctorService _proctorService;
    private readonly ITimeLimitedDataProtector _dataProtector;

    public ExamController(IProctorService proctorService, IDataProtectionProvider dataProtectionProvider)
    {
        _proctorService = proctorService;
        _dataProtector = dataProtectionProvider.CreateProtector("TestSession").ToTimeLimitedDataProtector();
    }

    [HttpPost("PreStart/Verify")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VerifyTestOutputViewModel))]
    public async Task<IActionResult> Verify(VerifyTestInput input)
    {
        var verifyResult = await _proctorService.VerifyTest(input);
        SetTestSession(new()
        {
            ProctorExamId = verifyResult.ProctorExamId,
            TestRunId = verifyResult.TestRunId,
            AccessCode = verifyResult.AccessCode,
            PreviousStep = PreStartSteps.VerifyTest,
            ClientProof = "some data",
            LifeTime = TimeSpan.FromMinutes(5)
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
        //todo: validate examinee info.

        var proctorExamActor = GetProctorActor(testSession);
        var examId = await proctorExamActor.ProvideExamineeInfo(new()
        {
            TestRunId = testSession.TestRunId,
            AccessCode = testSession.AccessCode,
            ExamineeInfo = data.ExamineeInfo
        });
        SetTestSession(new()
        {
            ProctorExamId = testSession.ProctorExamId,
            ExamId = examId,
            PreviousStep = PreStartSteps.ProvideExamineeInfo,
            ClientProof = testSession.ClientProof,
            //todo: discuss logic here.
            LifeTime = TimeSpan.FromMinutes(5)
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
            ExamId = testSession.ExamId
        });

        SetTestSession(new()
        {
            ProctorExamId = testSession.ProctorExamId,
            PreviousStep = PreStartSteps.Start,
            ClientProof = testSession.ClientProof,
            //todo: discuss logic here. Logical total duration + estimated delay time 5 mins.
            LifeTime = examContent.TotalDuration.Add(TimeSpan.FromMinutes(5))
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
    [ProducesResponseType(typeof(FinishExamOutput), 200)]
    public async Task<IActionResult> FinishExam()
    {
        var testSession = GetTestSession(PreStartSteps.FinishExam);
        var proctorExamActor = GetProctorActor(testSession);
        var result = await proctorExamActor.FinishExam();

        return Ok(result);
    }

    private static IProctorActor GetProctorActor(TestSession testSession)
    {
        return ActorProxy.Create<IProctorActor>(new ActorId(testSession.ProctorExamId), "ProctorActor");
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
                    if (session.PreviousStep != PreStartSteps.VerifyTest)
                    {
                        throw new TestPlatformException("InvalidStep");
                    }
                    break;
                }

            case PreStartSteps.Start:
                {
                    if (session.PreviousStep != PreStartSteps.ProvideExamineeInfo)
                    {
                        throw new TestPlatformException("InvalidStep");
                    }
                    break;
                }

            case PreStartSteps.SubmitAnswer:
            case PreStartSteps.FinishExam:
                {
                    if (session.PreviousStep != PreStartSteps.Start)
                    {
                        throw new TestPlatformException("InvalidStep");
                    }
                    break;
                }
        }

        return session;
    }

    private string EncryptTestSession(TestSession session)
    {
        return _dataProtector.Protect(JsonSerializer.Serialize(session), session.LifeTime);
    }

    private TestSession DecryptTestSession(string protectedSession)
    {
        try
        {
            var unprotectedSession = _dataProtector.Unprotect(protectedSession);
            return JsonSerializer.Deserialize<TestSession>(unprotectedSession) ?? throw new TestPlatformException("InvalidTestSession");
        }
        catch (Exception ex)
        {
            throw new TestPlatformException("InvalidTestSession", ex);
        }
    }
}

