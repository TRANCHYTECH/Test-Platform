using System;
using System.Threading.Tasks;
using Dapr.Actors;
using VietGeeks.TestPlatform.TestRunner.Contract;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlatform.TestRunner.Api.Actors;

public interface IProctorActor : IActor
{
    Task<StartExamOutput> StartExam(StartExamInput input);

    Task SubmitAnswer(SubmitAnswerInput input);

    Task<FinishExamOutput> FinishExam();
}
