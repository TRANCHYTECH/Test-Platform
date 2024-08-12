using AutoMapper;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.Integration.Contracts;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;
using Dapr.Client;
using VietGeeks.TestPlatform.Integration.Contract;
using shortid;
using FluentValidation;
using MongoDB.Entities;
using MongoDB.Driver;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class TestManagerService : ITestManagerService
{
    private readonly IValidator<TestDefinition> _testDefinitionValidator;
    private readonly DaprClient _daprClient;
    private readonly IMapper _mapper;
    private readonly TestManagerDbContext _managerDbContext;
    private readonly ServiceBusClient _bus;
    private readonly ILogger<TestManagerService> _logger;
    private readonly IClock _clock;

    public TestManagerService(
        IValidator<TestDefinition> testDefinitionValidator,
        DaprClient daprClient,
        IMapper mapper,
        TestManagerDbContext managerDbContext,
        ServiceBusClient bus,
        ILogger<TestManagerService> logger,
        IClock clock)
    {
        _testDefinitionValidator = testDefinitionValidator;
        _daprClient = daprClient;
        _mapper = mapper;
        _managerDbContext = managerDbContext;
        _bus = bus;
        _logger = logger;
        _clock = clock;
    }

    public async Task<TestDefinitionViewModel> CreateTestDefinition(NewTestDefinitionViewModel newTest)
    {
        var testEntity = _mapper.Map<TestDefinition>(newTest);
        // Assign default settings.
        //todo: some default values should be getted from db.
        testEntity.TestSetSettings = TestSetSettingsPart.Default();
        testEntity.TestAccessSettings = TestAccessSettingsPart.Default();
        testEntity.TestStartSettings = TestStartSettingsPart.Default();
        testEntity.GradingSettings = GradingSettingsPart.Default();
        testEntity.TimeSettings = TimeSettingsPart.Default();

        await _testDefinitionValidator.TryValidate(testEntity);

        await _managerDbContext.SaveAsync(testEntity);

        return _mapper.Map<TestDefinitionViewModel>(testEntity);
    }

    public async Task<TestDefinitionViewModel> GetTestDefinition(string id)
    {
        var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();

        return _mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<PagedSearchResult<TestDefinitionOverview>> GetTestDefinitionOverviews(int pageNumber, int pageSize, CancellationToken cancellationToken)
    {
        var entities = await _managerDbContext.PagedSearch<TestDefinition>()
            .Sort(c => c.CreatedOn, Order.Descending)
            .PageSize(pageSize)
            .PageNumber(pageNumber)
            .ProjectExcluding(c => new { c.TestAccessSettings, c.GradingSettings, c.TestSetSettings, c.TestStartSettings })
            .ExecuteAsync(cancellationToken);

        return new PagedSearchResult<TestDefinitionOverview>
        {
            Results = _mapper.Map<List<TestDefinitionOverview>>(entities.Results),
            TotalCount = entities.TotalCount,
            PageCount = entities.PageCount
        };
    }

    public async Task<TestDefinitionViewModel> UpdateTestDefinition(string id, UpdateTestDefinitionViewModel viewModel)
    {
        var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();
        if (entity == null)
            throw new EntityNotFoundException(id, nameof(TestDefinition));

        var updatedProperties = new List<string>();

        if (viewModel.BasicSettings != null)
        {
            entity.BasicSettings = _mapper.Map<TestBasicSettingsPart>(viewModel.BasicSettings);
            updatedProperties.Add(nameof(TestDefinition.BasicSettings));
        }

        if (viewModel.TestSetSettings != null)
        {
            entity.TestSetSettings = _mapper.Map<TestSetSettingsPart>(viewModel.TestSetSettings);
            updatedProperties.Add(nameof(TestDefinition.TestSetSettings));
        }

        if (viewModel.TestAccessSettings != null)
        {
            var updatedTestAccessSettings = _mapper.Map<TestAccessSettingsPart>(viewModel.TestAccessSettings);
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
            entity.TestAccessSettings = _mapper.Map<TestAccessSettingsPart>(viewModel.TestAccessSettings);

            updatedProperties.Add(nameof(TestDefinition.TestAccessSettings));
        }

        if (viewModel.GradingSettings != null)
        {
            entity.GradingSettings = _mapper.Map<GradingSettingsPart>(viewModel.GradingSettings);
            updatedProperties.Add(nameof(TestDefinition.GradingSettings));
        }

        if (viewModel.TimeSettings != null)
        {
            entity.TimeSettings = _mapper.Map<TimeSettingsPart>(viewModel.TimeSettings);
            updatedProperties.Add(nameof(TestDefinition.TimeSettings));
        }

        if (viewModel.TestStartSettings != null)
        {
            entity.TestStartSettings = _mapper.Map<TestStartSettingsPart>(viewModel.TestStartSettings);
            updatedProperties.Add(nameof(TestDefinition.TestStartSettings));
        }

        await _testDefinitionValidator.TryValidate(entity, updatedProperties.ToArray());

        if (updatedProperties.Count > 0)
        {
            var updateResult = await _managerDbContext.SaveOnlyAsync(entity, updatedProperties);
        }

        return _mapper.Map<TestDefinitionViewModel>(entity);
    }

    private static string[] GetPrivateAccessCodes(TestAccessSettings settings)
    {
        return ((PrivateAccessCodeType)settings).Configs.Select(c => c.Code).ToArray();
    }

    public async Task<TestDefinitionViewModel> ActivateTestDefinition(string id)
    {
        var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ?? throw new EntityNotFoundException(id, nameof(TestDefinition));
        var questions = await _managerDbContext.Find<QuestionDefinition>().ManyAsync(c => c.TestId == id);

        // Check if can activate test.
        var testRunTime = entity.EnsureCanActivate(questions.Count, _clock.UtcNow);

        using (var ctx = _managerDbContext.Transaction())
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
            var affectedFields = entity.Activate(testRun.ID, _clock.UtcNow, testRunTime.Status);

            await _managerDbContext.InsertAsync(testRun);
            await _managerDbContext.InsertAsync(testRunQuestionBatches);
            await _managerDbContext.SaveOnlyAsync(entity, affectedFields);

            await ctx.CommitTransactionAsync();
        }

        //todo: might make it loose couplely.
        await SendAccessCodes(entity);

        return _mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<TestDefinitionViewModel> EndTestDefinition(string id)
    {
        // Verify if status is in running.
        var validEntity = await _managerDbContext.Find<TestDefinition>().Match(c => c.ID == id && TestDefinition.ActiveStatuses.Contains(c.Status) && c.CurrentTestRun != null).ExecuteSingleAsync();
        if (validEntity == null)
        {
            throw new TestPlatformException("Not Found Activated/Scheduled Test Definition");
        }

        var testRun = await _managerDbContext.Find<TestRun>().MatchID(validEntity.CurrentTestRun?.Id).ExecuteSingleAsync();
        if (testRun == null)
        {
            throw new TestPlatformException("Not Found Test Run");
        }

        //todo: change to datetime.
        testRun.ExplicitEnd = true;
        testRun.TestDefinitionSnapshot = validEntity;

        var testDefFieldsAffected = validEntity.End();

        using (var ctx = _managerDbContext.Transaction())
        {
            await _managerDbContext.SaveOnlyAsync(validEntity, testDefFieldsAffected);
            //todo: improve performance by save affected parts.
            await _managerDbContext.SaveAsync(testRun);
            await ctx.CommitTransactionAsync();
        }

        return _mapper.Map<TestDefinitionViewModel>(validEntity);
    }

    public async Task<TestDefinitionViewModel> RestartTestDefinition(string id)
    {
        //todo: concurrency check impl.
        var testDef = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ?? throw new TestPlatformException("NotFoundEntity");

        var affectedFields = testDef.Restart();
        await _managerDbContext.SaveOnlyAsync(testDef, affectedFields);

        return _mapper.Map<TestDefinitionViewModel>(testDef);
    }

    public async Task<List<dynamic>> GetTestInvitationEvents(TestInvitationStatsInput input)
    {
        if (input == null)
        {
            throw new TestPlatformException("InvalidInput");
        }

        var keys = input.AccessCodes.Select(c => $"{input.TestDefinitionId}_{input.TestRunId}_{c}");
        var result = new List<dynamic>();
        var states = await _daprClient.GetBulkStateAsync("general-notify-store", keys.ToArray(), 2);
        foreach (var accessCode in input.AccessCodes)
        {
            var foundEvent = states.FirstOrDefault(c => c.Key.EndsWith(accessCode) && !string.IsNullOrEmpty(c.Value));
            if (foundEvent.Equals(default(BulkStateItem)))
            {
                continue;
            }

            var parsedEvents = JsonSerializer.Deserialize<TestInvitationEventData>(foundEvent.Value, _daprClient.JsonSerializerOptions);
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
            _logger.LogError("Could not send access code", ex);
        }
    }

    public async Task<dynamic> GenerateAccessCodes(string id, int quantity)
    {
        if (quantity <= 0)
            throw new TestPlatformException("Invalid argument quantity");

        // Get and verify the test definition.
        var testDef = await _managerDbContext.Find<TestDefinition>().MatchID(id)
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
        await _managerDbContext.SaveOnlyAsync(testDef, new[] { nameof(TestDefinition.TestAccessSettings) });

        return new
        {
            TestAccessSettings = _mapper.Map<TestAccessSettingsViewModel>(testDef.TestAccessSettings),
            ModifiedOn = testDef.ModifiedOn
        };
    }

    public async Task<dynamic> RemoveAccessCodes(string id, string[] codes)
    {
        //todo: reuse logic for 2 same methods.
        // Get and verify the test definition.
        var testDef = await _managerDbContext
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
        await _managerDbContext.SaveOnlyAsync(testDef, new[] { nameof(TestDefinition.TestAccessSettings) });

        return new
        {
            TestAccessSettings = _mapper.Map<TestAccessSettingsViewModel>(testDef.TestAccessSettings),
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
        var sender = _bus.CreateSender("send-test-access-code");
        await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(request)));
    }

    public async Task SendAccessCodes(string id, string[] codes)
    {
        //todo: reuse logic for 2 same methods.
        // Get and verify the test definition.
        var testDef = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ?? throw new EntityNotFoundException(id, nameof(TestDefinition));
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