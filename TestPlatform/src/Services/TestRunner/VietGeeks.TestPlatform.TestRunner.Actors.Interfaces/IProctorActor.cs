using System;
using System.Threading.Tasks;
using Dapr.Actors;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces.ViewModels;

namespace VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;

public interface IProctorActor : IActor
{
    Task StartExam(StartExamInput input);
}
