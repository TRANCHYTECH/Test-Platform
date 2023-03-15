using System;
using System.Threading.Tasks;
using Dapr.Actors;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces.Contracts;

namespace VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;

public interface IProctorActor : IActor
{
    Task StartExam(StartExamInput input);

    Task SubmitAnswer(SubmitAnswerInput input);
}
