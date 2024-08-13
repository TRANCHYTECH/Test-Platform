using System.Text.Json;
using AutoMapper;
using Azure.Messaging.ServiceBus;
using Dapr.Client;
using FluentValidation;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using shortid;
using VietGeeks.TestPlatform.Integration.Contract;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services;

public class TestManagerService(
    IValidator<TestDefinition> testDefinitionValidator,
    DaprClient daprClient,
    IMapper mapper,
    TestManagerDbContext managerDbContext,
    ServiceBusClient bus,
    ILogger<TestManagerService> logger,
    IClock clock)
    : ITestManagerService
{
    public async Task<TestDefinitionViewModel> CreateTestDefinition(NewTestDefinitionViewModel newTest)
    {
        var testEntity = mapper.Map<TestDefinition>(newTest);
        // Assign default settings.
        //todo: some default values should be getted from db.
        testEntity.TestSetSettings = TestSetSettingsPart.Default();
        testEntity.TestAccessSettings = TestAccessSettingsPart.Default();
        testEntity.TestStartSettings = TestStartSettingsPart.Default();
        testEntity.GradingSettings = GradingSettingsPart.Default();
        testEntity.TimeSettings = TimeSettingsPart.Default();

        await testDefinitionValidator.TryValidate(testEntity);

        await managerDbContext.SaveAsync(testEntity);

        return mapper.Map<TestDefinitionViewModel>(testEntity);
    }

    public async Task<TestDefinitionViewModel> GetTestDefinition(string id)
    {
        var entity = await managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();

        return mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<PagedSearchResult<TestDefinitionOverview>> GetTestDefinitionOverviews(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await managerDbContext.PagedSearch<TestDefinition>()
            .Sort(c => c.CreatedOn, Order.Descending)
            .PageSize(pageSize)
            .PageNumber(pageNumber)
            .ProjectExcluding(c => new { c.TestAccessSettings, c.GradingSettings, c.TestSetSettings, c.TestStartSettings })
            .ExecuteAsync(cancellationToken);

        return new PagedSearchResult<TestDefinitionOverview>
        {
            Results = mapper.Map<List<TestDefinitionOverview>>(entities.Results),
            TotalCount = entities.TotalCount,
            PageCount = entities.PageCount
        };
    }

    public async Task<TestDefinitionViewModel> UpdateTestDefinition(string id, UpdateTestDefinitionViewModel viewModel)
    {
        var entity = await managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();
        if (entity == null)
            throw new EntityNotFoundException(id, nameof(TestDefinition));

        var updatedProperties = new List<string>();

        if (viewModel.BasicSettings != null)
        {
            entity.BasicSettings = mapper.Map<TestBasicSettingsPart>(viewModel.BasicSettings);
            updatedProperties.Add(nameof(TestDefinition.BasicSettings));
        }

        if (viewModel.TestSetSettings != null)
        {
            entity.TestSetSettings = mapper.Map<TestSetSettingsPart>(viewModel.TestSetSettings);
            updatedProperties.Add(nameof(TestDefinition.TestSetSettings));
        }

        if (viewModel.TestAccessSettings != null)
        {
            var updatedTestAccessSettings = mapper.Map<TestAccessSettingsPart>(viewModel.TestAccessSettings);
            // BUSINESS: User are not allowed to change access codes with this flow. So we have to verify input codes match exactly with existing.
            if (updatedTestAccessSettings.AccessType == TestAcessType.PrivateAccessCode && entity.TestAccessSettings.AccessType == TestAcessType.PrivateAccessCode)
            {
                var currentCodes = GetPrivateAccessCodes(entity.TestAccessSettings.Settings);
                var updatedCodes = GetPrivateAccessCodes(updatedTestAccessSettings.Settings);
                if (currentCodes.Count() != updatedCodes.Count() || currentCodes.Except(updatedCodes).Count() > 0)
                {
                    throw new TestPlatformException("Mismatched Access Codes");
                }
            }
            entity.TestAccessSettings = mapper.Map<TestAccessSettingsPart>(viewModel.TestAccessSettings);

            updatedProperties.Add(nameof(TestDefinition.TestAccessSettings));
        }

        if (viewModel.GradingSettings != null)
        {
            entity.GradingSettings = mapper.Map<GradingSettingsPart>(viewModel.GradingSettings);
            updatedProperties.Add(nameof(TestDefinition.GradingSettings));
        }

        if (viewModel.TimeSettings != null)
        {
            entity.TimeSettings = mapper.Map<TimeSettingsPart>(viewModel.TimeSettings);
            updatedProperties.Add(nameof(TestDefinition.TimeSettings));
        }

        if (viewModel.TestStartSettings != null)
        {
            entity.TestStartSettings = mapper.Map<TestStartSettingsPart>(viewModel.TestStartSettings);
            updatedProperties.Add(nameof(TestDefinition.TestStartSettings));
        }

        await testDefinitionValidator.TryValidate(entity, updatedProperties.ToArray());

        if (updatedProperties.Count > 0)
        {
            var updateResult = await managerDbContext.SaveOnlyAsync(entity, updatedProperties);
        }

        return mapper.Map<TestDefinitionViewModel>(entity);
    }

    private static string[] GetPrivateAccessCodes(TestAccessSettings settings)
    {
        return ((PrivateAccessCodeType)settings).Configs.Select(c => c.Code).ToArray();
    }

    public async Task<TestDefinitionViewModel> ActivateTestDefinition(string id)
    {
        var entity = await managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ?? throw new EntityNotFoundException(id, nameof(TestDefinition));
        var questions = await managerDbContext.Find<QuestionDefinition>().ManyAsync(c => c.TestId == id);

        // Check if can activate test.
        var testRunTime = entity.EnsureCanActivate(questions.Count, clock.UtcNow);

        using (var ctx = managerDbContext.Transaction())
        {
            // Create test run.
            var testRun = new TestRun
            {
                ID = ObjectId.GenerateNewId().ToString(),
                StartAtUtc = testRunTime.StartAtUtc,
                EndAtUtc = testRunTime.EndAtUtc,
                TestDefinitionSnapshot = entity
            };

            // Copy questions.
            var testRunQuestionBatches = questions.Chunk(10).Select(q => new TestRunQuestion
            {
                TestRunId = testRun.ID,
                Batch = q.ToArray()
            });

            // Set current activated test run.
            var affectedFields = entity.Activate(testRun.ID, clock.UtcNow, testRunTime.Status);

            await managerDbContext.InsertAsync(testRun);
            await managerDbContext.InsertAsync(testRunQuestionBatches);
            await managerDbContext.SaveOnlyAsync(entity, affectedFields);

            await ctx.CommitTransactionAsync();
        }

        //todo: might make it loose couplely.
        await SendAccessCodes(entity);

        return mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<TestDefinitionViewModel> EndTestDefinition(string id)
    {
        // Verify if status is in running.
        var validEntity = await managerDbContext.Find<TestDefinition>().Match(c => c.ID == id && TestDefinition.ActiveStatuses.Contains(c.Status) && c.CurrentTestRun != null).ExecuteSingleAsync();
        if (validEntity == null)
        {
            throw new TestPlatformException("Not Found Activated/Scheduled Test Definition");
        }

        var testRun = await managerDbContext.Find<TestRun>().MatchID(validEntity.CurrentTestRun?.Id).ExecuteSingleAsync();
        if (testRun == null)
        {
            throw new TestPlatformException("Not Found Test Run");
        }

        //todo: change to datetime.
        testRun.ExplicitEnd = true;
        testRun.TestDefinitionSnapshot = validEntity;

        var testDefFieldsAffected = validEntity.End();

        using (var ctx = managerDbContext.Transaction())
        {
            await managerDbContext.SaveOnlyAsync(validEntity, testDefFieldsAffected);
            //todo: improve performance by save affected parts.
            await managerDbContext.SaveAsync(testRun);
            await ctx.CommitTransactionAsync();
        }

        return mapper.Map<TestDefinitionViewModel>(validEntity);
    }

    public async Task<TestDefinitionViewModel> RestartTestDefinition(string id)
    {
        //todo: concurrency check impl.
        var testDef = await managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ?? throw new TestPlatformException("NotFoundEntity");

        var affectedFields = testDef.Restart();
        await managerDbContext.SaveOnlyAsync(testDef, affectedFields);

        return mapper.Map<TestDefinitionViewModel>(testDef);
    }

    public async Task<List<dynamic>> GetTestInvitationEvents(TestInvitationStatsInput input)
    {
        if (input == null)
        {
            throw new TestPlatformException("InvalidInput");
        }

        var keys = input.AccessCodes.Select(c => $"{input.TestDefinitionId}_{input.TestRunId}_{c}");
        var result = new List<dynamic>();
        var states = await daprClient.GetBulkStateAsync("general-notify-store", keys.ToArray(), 2);
        foreach (var accessCode in input.AccessCodes)
        {
            var foundEvent = states.FirstOrDefault(c => c.Key.EndsWith(accessCode) && !string.IsNullOrEmpty(c.Value));
            if (foundEvent.Equals(default(BulkStateItem)))
            {
                continue;
            }

            var parsedEvents = JsonSerializer.Deserialize<TestInvitationEventData>(foundEvent.Value, daprClient.JsonSerializerOptions);
            if (parsedEvents == null)
            {
                continue;
            }
            if (parsedEvents.Events == null)
            {
                continue;
            }

            result.Add(new
            {
                AccessCode = accessCode,
                parsedEvents.Events
            });
        }

        return result;
    }

    private async Task SendAccessCodes(TestDefinition entity)
    {
        try
        {
            if (entity != null && entity.CurrentTestRun != null && (entity.TestAccessSettings?.Settings is PrivateAccessCodeType config))
            {
                var receivers = config.Configs.Where(c => !string.IsNullOrEmpty(c.Email) && c.SendCode).Select(c => new Receiver(c.Code, c.Email)).ToArray();
                await ProcessSendAccessCodes(entity, receivers);
            }
        }
        catch (Exception ex)
        {
            logger.LogError("Could not send access code", ex);
        }
    }

    public async Task<dynamic> GenerateAccessCodes(string id, int quantity)
    {
        if (quantity <= 0)
            throw new TestPlatformException("Invalid argument quantity");

        // Get and verify the test definition.
        var testDef = await managerDbContext.Find<TestDefinition>().MatchID(id)
        .Project(c => c
            .Include(nameof(TestDefinition.Status))
            .Include(nameof(TestDefinition.ModifiedOn))
            .Include(nameof(TestDefinition.CreatedOn))
            .Include(nameof(TestDefinition.TimeSettings))
            .Include(nameof(TestDefinition.TestAccessSettings))
            .Include(nameof(TestDefinition.CurrentTestRun)))
        .ExecuteFirstAsync() ?? throw new EntityNotFoundException(id, nameof(TestDefinition));
        if (testDef.TestAccessSettings.AccessType != TestAcessType.PrivateAccessCode)
        {
            throw new TestPlatformException("Invalid entity");
        }

        if (testDef.LatestStatus == Data.Models.TestDefinitionStatus.Ended)
        {
            throw new TestPlatformException("Invalid status");
        }

        if (testDef.TestAccessSettings.Settings is PrivateAccessCodeType privateAccessCodeSettings == false)
        {
            throw new TestPlatformException("Invalid settings");
        }

        // Generate access codes.
        var accessCodes = new List<string>();
        for (int i = 0; i < quantity; i++)
        {
            var accessCode = ShortId.Generate(new(true, false, 10));
            privateAccessCodeSettings.Configs.Add(new()
            {
                Code = accessCode
            });
            accessCodes.Add(accessCode);
        }

        // Update affected property: TestAccessSettings.
        await managerDbContext.SaveOnlyAsync(testDef, new[] { nameof(TestDefinition.TestAccessSettings) });

        return new
        {
            TestAccessSettings = mapper.Map<TestAccessSettingsViewModel>(testDef.TestAccessSettings),
            ModifiedOn = testDef.ModifiedOn
        };
    }

    public async Task<dynamic> RemoveAccessCodes(string id, string[] codes)
    {
        //todo: reuse logic for 2 same methods.
        // Get and verify the test definition.
        var testDef = await managerDbContext
        .Find<TestDefinition>()
        .MatchID(id)
        .Project(c => c
            .Include(nameof(TestDefinition.Status))
            .Include(nameof(TestDefinition.ModifiedOn))
            .Include(nameof(TestDefinition.CreatedOn))
            .Include(nameof(TestDefinition.TimeSettings))
            .Include(nameof(TestDefinition.TestAccessSettings))
            .Include(nameof(TestDefinition.CurrentTestRun)))
        .ExecuteFirstAsync() ?? throw new EntityNotFoundException(id, nameof(TestDefinition));
        if (testDef.TestAccessSettings.AccessType != TestAcessType.PrivateAccessCode)
        {
            throw new TestPlatformException("Invalid entity");
        }

        if (testDef.LatestStatus == Data.Models.TestDefinitionStatus.Ended)
        {
            throw new TestPlatformException("Invalid status");
        }

        if (testDef.TestAccessSettings.Settings is PrivateAccessCodeType privateAccessCodeSettings == false)
        {
            throw new TestPlatformException("Invalid settings");
        }

        foreach (var code in codes)
        {
            var existingAccessCode = privateAccessCodeSettings.Configs.SingleOrDefault(c => c.Code == code);
            if (existingAccessCode == null)
                throw new TestPlatformException("Invalid access code");

            privateAccessCodeSettings.Configs.Remove(existingAccessCode);
        }

        // Update affected property: TestAccessSettings.
        await managerDbContext.SaveOnlyAsync(testDef, new[] { nameof(TestDefinition.TestAccessSettings) });

        return new
        {
            TestAccessSettings = mapper.Map<TestAccessSettingsViewModel>(testDef.TestAccessSettings),
            ModifiedOn = testDef.ModifiedOn
        };
    }

    private async Task ProcessSendAccessCodes(TestDefinition entity, Receiver[] receivers)
    {
        if (receivers.Length == 0)
        {
            return;
        }

        var request = new SendTestAccessCodeRequest
        {
            //todo: configure test url.
            TestUrl = $"https://dev.test-runner.testmaster.io/start/{entity.ID}",
            TestDefinitionId = entity.ID!,
            TestRunId = entity.CurrentTestRun?.Id!,
            Receivers = receivers
        };

        //todo: prevent harded code here.
        var sender = bus.CreateSender("send-test-access-code");
        await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(request)));
    }

    public async Task SendAccessCodes(string id, string[] codes)
    {
        //todo: reuse logic for 2 same methods.
        // Get and verify the test definition.
        var testDef = await managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ?? throw new EntityNotFoundException(id, nameof(TestDefinition));
        if (testDef.TestAccessSettings.AccessType != TestAcessType.PrivateAccessCode)
        {
            throw new TestPlatformException("Invalid entity");
        }

        if (testDef.LatestStatus == Data.Models.TestDefinitionStatus.Ended)
        {
            throw new TestPlatformException("Invalid status");
        }

        if (testDef.TestAccessSettings.Settings is PrivateAccessCodeType privateAccessCodeSettings == false)
        {
            throw new TestPlatformException("Invalid settings");
        }

        var receivers = new List<Receiver>();
        foreach (var code in codes)
        {
            var existingAccessCode = privateAccessCodeSettings.Configs.SingleOrDefault(c => c.Code == code);
            if (existingAccessCode == null)
                throw new TestPlatformException("Invalid access code");

            if (string.IsNullOrWhiteSpace(existingAccessCode.Email))
                throw new TestPlatformException("Invalid email");

            receivers.Add(new()
            {
                Email = existingAccessCode.Email,
                AccessCode = existingAccessCode.Code
            });
        }

        await ProcessSendAccessCodes(testDef, receivers.ToArray());
    }
}