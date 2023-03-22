using AutoMapper;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.Integration.Contracts;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class TestManagerService : ITestManagerService
{
    private readonly IMapper _mapper;
    private readonly TestManagerDbContext _managerDbContext;
    private readonly ServiceBusClient _bus;

    public TestManagerService(IMapper mapper, TestManagerDbContext managerDbContext, ServiceBusClient bus)
    {
        _mapper = mapper;
        _managerDbContext = managerDbContext;
        _bus = bus;
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

        await _managerDbContext.SaveAsync(testEntity);

        return _mapper.Map<TestDefinitionViewModel>(testEntity);
    }

    public async Task<TestDefinitionViewModel> GetTestDefinition(string id)
    {
        var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();

        return _mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<List<TestDefinitionViewModel>> GetTestDefinitions()
    {
        var entities = await _managerDbContext.Find<TestDefinition>().ExecuteAsync();

        return _mapper.Map<List<TestDefinitionViewModel>>(entities);
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

        if (updatedProperties.Count > 0)
        {
            var updateResult = await _managerDbContext.SaveOnlyAsync(entity, updatedProperties);
        }

        return _mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<List<TestCategoryViewModel>> GetTestCategories()
    {
        var entities = await _managerDbContext.Find<TestCategory>().ManyAsync(c => c is TestCategory);

        return _mapper.Map<List<TestCategoryViewModel>>(entities);
    }

    public async Task<TestCategoryViewModel> CreateTestCategory(NewTestCategoryViewModel newTestCategory)
    {
        var testCategoryEntity = _mapper.Map<TestCategory>(newTestCategory);
        await _managerDbContext.SaveAsync(testCategoryEntity);

        return _mapper.Map<TestCategoryViewModel>(testCategoryEntity);
    }

    public async Task<TestDefinitionViewModel> ActivateTestDefinition(string id)
    {
        // Find test definition
        var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ?? throw new EntityNotFoundException(id, nameof(TestDefinition));

        // Check status
        if (!entity.CanActivate)
        {
            throw new TestPlatformException("Test not ready for activation");
        }

        // Combine with checking questions.
        var questionCount = await _managerDbContext.CountAsync<QuestionDefinition>(c => c.TestId == id);
        if(questionCount == 0)
        {
            throw new TestPlatformException("Test not ready for activation");
        }

        entity.Activate();
        await _managerDbContext.SaveOnlyAsync(entity, new[] { nameof(TestDefinition.Status) });

        // Send access code invitations.
        if (entity.TestAccessSettings?.AccessType == TestAcessType.PrivateAccessCode && entity.TestAccessSettings.Settings != null)
        {
            var settings = (PrivateAccessCodeType)entity.TestAccessSettings.Settings;
            var receivers = settings.Configs.Where(c => !string.IsNullOrEmpty(c.Email)).Select(c => new Receiver(c.Code, c.Email)).ToArray();
            if (receivers.Length == 0)
            {
                return _mapper.Map<TestDefinitionViewModel>(entity);
            }

            var request = new SendTestAccessCodeRequest
            {
                //todo: configure test url.
                TestUrl = $"https://dev.test-runner.testmaster.io/start/{id}",
                TestDefinitionId = id,
                Receivers = receivers
            };

            var sender = _bus.CreateSender("send-test-access-code");
            await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(request)));
        }

        return _mapper.Map<TestDefinitionViewModel>(entity);
    }

    public async Task<ReadyForActivationStatus> CheckTestDefinitionReadyForActivation(string id)
    {
        var validEntity = await _managerDbContext.Find<TestDefinition>().Match(c => c.ID == id && c.Status == Core.Models.TestDefinitionStatus.Draft &&
        c.TimeSettings != null &&
        c.TestSetSettings != null &&
        c.TestAccessSettings != null &&
        c.GradingSettings != null &&
        c.TestStartSettings != null).IgnoreGlobalFilters().Project(c => new TestDefinition { ID = c.ID }).ExecuteFirstAsync();
        if (validEntity == null)
        {
            return new()
            {
                IsReady = false,
                Errors = new[] { "NotFoundEntity" }
            };
        }

        var questionCount = await _managerDbContext.CountAsync<QuestionDefinition>(c => c.TestId == id, ignoreGlobalFilters: true);
        if (questionCount == 0)
        {
            return new()
            {
                IsReady = false,
                Errors = new[] { "QuestionMissing" }
            };
        }

        return new()
        {
            IsReady = true
        };
    }
}