using Dapr.Actors.Runtime;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces.ViewModels;
using VietGeeks.TestPlatform.TestRunner.Contract;

namespace VietGeeks.TestPlatform.TestRunner.Api.Actors;

public class ProctorActor : Actor, IProctorActor
{
    public ProctorActor(ActorHost host) : base(host)
    {
    }

    public async Task StartExam(StartExamInput input)
    {
        await StateManager.SetStateAsync("input", input.TestDefinitionId);
    }
}
