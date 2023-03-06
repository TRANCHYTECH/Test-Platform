using Dapr.Actors.Runtime;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces;
using VietGeeks.TestPlatform.TestRunner.Actors.Interfaces.ViewModels;

namespace VietGeeks.TestPlatform.TestRunner.Api.Actors;

public class ProctorActor : Actor, IProctorActor
{
    public ProctorActor(ActorHost host) : base(host)
    {
    }

    public async Task<TestViewModel> StartTest(string testCode)
    {
        if (await StateManager.ContainsStateAsync(testCode))
        {
            var id = await StateManager.GetStateAsync<string>(testCode);
            return new TestViewModel { Id = id };
        }

        var newId = Guid.NewGuid().ToString();
        await StateManager.SetStateAsync(testCode, newId);
        return new TestViewModel { Id = newId };
    }

    public Task<TestViewModel> SubmitTest(string testCode)
    {
        return Task.FromResult(new TestViewModel { Id = Guid.NewGuid().ToString() });
    }
}
