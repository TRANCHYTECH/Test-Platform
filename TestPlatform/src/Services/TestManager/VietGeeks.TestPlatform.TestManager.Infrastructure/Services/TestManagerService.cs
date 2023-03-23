﻿using AutoMapper;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.Integration.Contracts;
using Azure.Messaging.ServiceBus;
using System.Text.Json;
using MongoDB.Bson;
using Microsoft.Extensions.Logging;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure;

public class TestManagerService : ITestManagerService
{
    private readonly IMapper _mapper;
    private readonly TestManagerDbContext _managerDbContext;
    private readonly ServiceBusClient _bus;
    private readonly ILogger<TestManagerService> _logger;

    public TestManagerService(IMapper mapper, TestManagerDbContext managerDbContext, ServiceBusClient bus, ILogger<TestManagerService> logger)
    {
        _mapper = mapper;
        _managerDbContext = managerDbContext;
        _bus = bus;
        _logger = logger;
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
        var entity = await _managerDbContext.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ?? throw new EntityNotFoundException(id, nameof(TestDefinition));
        var questions = await _managerDbContext.Find<QuestionDefinition>().ManyAsync(c => c.TestId == id);

        // Check if can activate test.
        var testRunTime = entity.EnsureCanActivate(questions.Count);

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
            entity.Activate(testRun.ID);

            await _managerDbContext.InsertAsync(testRun);
            await _managerDbContext.InsertAsync(testRunQuestionBatches);
            await _managerDbContext.SaveOnlyAsync(entity, new[] { nameof(TestDefinition.Status), nameof(TestDefinition.CurrentTestRun) });

            await ctx.CommitTransactionAsync();
        }

        //todo: might make it loose couplely.
        await SendAccessCodes(entity);

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

    private async Task SendAccessCodes(TestDefinition entity)
    {
        try
        {
            if ((entity.TestAccessSettings?.Settings is PrivateAccessCodeType config))
            {
                var receivers = config.Configs.Where(c => !string.IsNullOrEmpty(c.Email)).Select(c => new Receiver(c.Code, c.Email)).ToArray();
                if (receivers.Length > 0)
                {
                    var request = new SendTestAccessCodeRequest
                    {
                        //todo: configure test url.
                        TestUrl = $"https://dev.test-runner.testmaster.io/start/{entity.ID}",
                        TestDefinitionId = entity.ID,
                        Receivers = receivers
                    };

                    var sender = _bus.CreateSender("send-test-access-code");
                    await sender.SendMessageAsync(new ServiceBusMessage(JsonSerializer.Serialize(request)));
                }
            }
        }
        catch (Exception ex)
        {
            _logger.LogError("Could not send access code", ex);
        }
    }
}