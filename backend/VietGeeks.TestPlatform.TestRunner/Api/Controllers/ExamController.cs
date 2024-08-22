using System.Net;
using System.Text.Json;
using AutoMapper;
using Dapr.Actors;
using Dapr.Actors.Client;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Mvc;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestRunner.Api.Actors;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;
using VietGeeks.TestPlatform.TestRunner.Infrastructure.Services;

namespace VietGeeks.TestPlatform.TestRunner.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ExamController(
    IProctorService proctorService,
    IDataProtectionProvider dataProtectionProvider,
    IMapper mapper)
    : ControllerBase
{
    private readonly ITimeLimitedDataProtector _dataProtector =
        dataProtectionProvider.CreateProtector("TestSession").ToTimeLimitedDataProtector();

    [HttpPost("PreStart/Verify")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(VerifyTestOutputViewModel))]
    [ProducesResponseType((int)HttpStatusCode.BadRequest, Type = typeof(ErrorDetails))]
    public async Task<IActionResult> Verify(VerifyTestInput input)
    {
        var verifyResult = await proctorService.VerifyTest(input);
        SetTestSession(new TestSession
        {
            ProctorExamId = verifyResult.ProctorExamId,
            TestRunId = verifyResult.TestRunId,
            AccessCode = verifyResult.AccessCode,
            PreviousStep = ExamStep.VerifyTest,
            ClientProof = "some data",
            LifeTime = TimeSpan.FromMinutes(5)
        });

        return Ok(new VerifyTestOutputViewModel
        {
            AccessCode = verifyResult.AccessCode,
            TestName = verifyResult.TestName,
            ConsentMessage = verifyResult.ConsentMessage,
            InstructionMessage = verifyResult.InstructionMessage
        });
    }

    [HttpPost("PreStart/ProvideExamineeInfo")]
    [ProducesResponseType((int)HttpStatusCode.OK, Type = typeof(ProvideExamineeInfoOutput))]
    public async Task<IActionResult> ProvideExamineeInfo(ProvideExamineeInfoViewModel data)
    {
        var testSession = GetTestSession(ExamStep.ProvideExamineeInfo);
        //todo: validate examinee info.

        var proctorExamActor = GetProctorActor(testSession);
        var examId = await proctorExamActor.ProvideExamineeInfo(new ProvideExamineeInfoInput
        {
            TestRunId = testSession.TestRunId,
            AccessCode = testSession.AccessCode,
            ExamineeInfo = data.ExamineeInfo
        });
        SetTestSession(new TestSession
        {
            ProctorExamId = testSession.ProctorExamId,
            ExamId = examId,
            PreviousStep = ExamStep.ProvideExamineeInfo,
            ClientProof = testSession.ClientProof,
            //todo: discuss logic here.
            LifeTime = TimeSpan.FromMinutes(5)
        });

        return Ok();
    }

    [HttpPost("Start")]
    [ProducesResponseType(typeof(StartExamOutputViewModel), 200)]
    public async Task<IActionResult> StartExam()
    {
        var testSession = GetTestSession(ExamStep.Start);
        var proctorExamActor = GetProctorActor(testSession);
        var examContent = await proctorExamActor.StartExam(new StartExamInput
        {
            ExamId = testSession.ExamId
        });

        SetTestSession(new TestSession
        {
            ProctorExamId = testSession.ProctorExamId,
            ExamId = testSession.ExamId,
            PreviousStep = ExamStep.Start,
            ClientProof = testSession.ClientProof,
            //todo: discuss logic here. Logical total duration + estimated delay time 5 mins.
            LifeTime = examContent.TotalDuration.Add(TimeSpan.FromMinutes(5))
        });

        var output = mapper.Map<StartExamOutputViewModel>(examContent);
        output.Step = ExamStep.Start;

        return Ok(output);
    }

    [HttpPost("SubmitAnswer")]
    [ProducesResponseType(typeof(SubmitAnswerOutput), 200)]
    public async Task<IActionResult> SubmitAnswer(SubmitAnswerViewModel data)
    {
        var testSession = GetTestSession(ExamStep.SubmitAnswer);
        var proctorExamActor = GetProctorActor(testSession);
        var output = await proctorExamActor.SubmitAnswer(new SubmitAnswerInput
        {
            QuestionId = data.QuestionId,
            AnswerIds = data.AnswerIds
        });

        return Ok(output);
    }

    [HttpPost("PreviousQuestion")]
    [ProducesResponseType(typeof(ActivateQuestionOutput), 200)]
    public async Task<IActionResult> PreviousQuestion()
    {
        var testSession = GetTestSession(ExamStep.SubmitAnswer);
        var proctorExamActor = GetProctorActor(testSession);
        var output = await proctorExamActor.ActivateQuestionFromCurrent(new ActivateNextQuestionInput
        {
            Direction = ActivateDirection.Previous
        });

        return Ok(output);
    }

    [HttpPost("NextQuestion")]
    [ProducesResponseType(typeof(ActivateQuestionOutput), 200)]
    public async Task<IActionResult> NextQuestion()
    {
        var testSession = GetTestSession(ExamStep.SubmitAnswer);
        var proctorExamActor = GetProctorActor(testSession);
        var output = await proctorExamActor.ActivateQuestionFromCurrent(new ActivateNextQuestionInput
        {
            Direction = ActivateDirection.Next
        });

        return Ok(output);
    }

    [HttpPost("GoToQuestion")]
    [ProducesResponseType(typeof(ActivateQuestionOutput), 200)]
    public async Task<IActionResult> GoToQuestion(int questionIndex)
    {
        var testSession = GetTestSession(ExamStep.SubmitAnswer);
        var proctorExamActor = GetProctorActor(testSession);
        var output = await proctorExamActor.ActivateQuestionByIndex(questionIndex);

        return Ok(output);
    }

    [HttpPost("Finish")]
    [ProducesResponseType(typeof(FinishExamOutput), 200)]
    public async Task<IActionResult> FinishExam()
    {
        var testSession = GetTestSession(ExamStep.FinishExam);
        var proctorExamActor = GetProctorActor(testSession);
        var result = await proctorExamActor.FinishExam();

        SetTestSession(new TestSession
        {
            ProctorExamId = testSession.ProctorExamId,
            ExamId = testSession.ExamId,
            PreviousStep = ExamStep.FinishExam,
            ClientProof = testSession.ClientProof,
            LifeTime = TimeSpan.FromMinutes(5)
        });

        return Ok(result);
    }

    [HttpGet("AfterTestConfig")]
    [ProducesResponseType(typeof(AfterTestConfigOutput), 200)]
    public async Task<IActionResult> GetAfterTestConfigs()
    {
        var testSession = GetTestSession(ExamStep.FinishExam);

        var output = await proctorService.GetAfterTestConfigAsync(testSession?.ExamId);

        return Ok(output);
    }

    [HttpGet("Status")]
    [ProducesResponseType(typeof(ExamStatusWithStep), 200)]
    public async Task<IActionResult> GetExamStatus()
    {
        // TODO: verify clientProof?
        var testSession = GetTestSession();

        if (testSession == null)
        {
            return NotFound();
        }

        var proctorExamActor = GetProctorActor(testSession);
        var examStatus = await proctorExamActor.GetExamStatus();
        var result = mapper.Map<ExamStatusWithStep>(examStatus);

        result.Step = testSession.PreviousStep;

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

    private TestSession? GetTestSession(ExamStep forStep)
    {
        var session = GetTestSession();
        ValidateStep(forStep, session);

        return session;
    }

    private TestSession GetTestSession()
    {
        var header = Request.Headers[nameof(TestSession)];
        var session = DecryptTestSession(header.ToString());

        return session;
    }

    private static void ValidateStep(ExamStep forStep, TestSession? session)
    {
        if (session == null)
        {
            throw new TestPlatformException("NoSession");
        }

        switch (forStep)
        {
            case ExamStep.ProvideExamineeInfo:
            {
                if (session.PreviousStep != ExamStep.VerifyTest)
                {
                    throw new TestPlatformException("InvalidStep");
                }

                break;
            }

            case ExamStep.Start:
            {
                if (session.PreviousStep != ExamStep.ProvideExamineeInfo)
                {
                    throw new TestPlatformException("InvalidStep");
                }

                break;
            }

            case ExamStep.SubmitAnswer:
            {
                if (session.PreviousStep != ExamStep.Start)
                {
                    throw new TestPlatformException("InvalidStep");
                }

                break;
            }
        }
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
            return JsonSerializer.Deserialize<TestSession>(unprotectedSession) ??
                   throw new TestPlatformException("InvalidTestSession");
        }
        catch (Exception ex)
        {
            throw new TestPlatformException("InvalidTestSession", ex);
        }
    }
}