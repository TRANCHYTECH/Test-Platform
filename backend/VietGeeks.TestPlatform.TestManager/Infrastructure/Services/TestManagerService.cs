using System.Text.Json;
using AutoMapper;
using Dapr.Client;
using FluentValidation;
using MassTransit;
using MassTransit.MongoDbIntegration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Entities;
using shortid;
using shortid.Configuration;
using VietGeeks.TestPlatform.Integration.Contract;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.SharedKernel.PureServices;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators;
using TestDefinitionStatus = VietGeeks.TestPlatform.TestManager.Data.Models.TestDefinitionStatus;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.Services
{
    public class TestManagerService(
        IValidator<TestDefinition> testDefinitionValidator,
        DaprClient daprClient,
        IMapper mapper,
        MongoDbContext dbContext,
        IPublishEndpoint publishEndpoint,
        ILogger<TestManagerService> logger,
        IConfiguration configuration,
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

            await DB.SaveAsync(testEntity);

            return mapper.Map<TestDefinitionViewModel>(testEntity);
        }

        public async Task<TestDefinitionViewModel> GetTestDefinition(string id)
        {
            var entity = await DB.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();

            return mapper.Map<TestDefinitionViewModel>(entity);
        }

        public async Task<PagedSearchResult<TestDefinitionOverview>> GetTestDefinitionOverviews(int pageNumber,
            int pageSize, CancellationToken cancellationToken)
        {
            var entities = await DB.PagedSearch<TestDefinition>()
                .Sort(c => c.CreatedOn, Order.Descending)
                .PageSize(pageSize)
                .PageNumber(pageNumber)
                .ProjectExcluding(c => new
                    { c.TestAccessSettings, c.GradingSettings, c.TestSetSettings, c.TestStartSettings })
                .ExecuteAsync(cancellationToken);

            return new PagedSearchResult<TestDefinitionOverview>
            {
                Results = mapper.Map<List<TestDefinitionOverview>>(entities.Results),
                TotalCount = entities.TotalCount,
                PageCount = entities.PageCount
            };
        }

        public async Task<TestDefinitionViewModel> UpdateTestDefinition(string id,
            UpdateTestDefinitionViewModel viewModel)
        {
            var entity = await DB.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync();
            if (entity == null)
            {
                throw new EntityNotFoundException(id, nameof(TestDefinition));
            }

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
                if (updatedTestAccessSettings.AccessType == TestAcessType.PrivateAccessCode &&
                    entity.TestAccessSettings.AccessType == TestAcessType.PrivateAccessCode)
                {
                    var currentCodes = GetPrivateAccessCodes(entity.TestAccessSettings.Settings);
                    var updatedCodes = GetPrivateAccessCodes(updatedTestAccessSettings.Settings);
                    if (currentCodes.Length != updatedCodes.Length || currentCodes.Except(updatedCodes).Any())
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
                var updateResult = await DB.SaveOnlyAsync(entity, updatedProperties);
            }

            return mapper.Map<TestDefinitionViewModel>(entity);
        }

        public async Task<TestDefinitionViewModel> ActivateTestDefinition(string id,
            CancellationToken cancellationToken)
        {
            var entity = await DB.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync(cancellationToken) ??
                         throw new EntityNotFoundException(id, nameof(TestDefinition));
            var questions = await DB.Find<QuestionDefinition>().ManyAsync(c => c.TestId == id, cancellationToken);

            // Check if can activate test.
            var testRunTime = entity.EnsureCanActivate(questions.Count, clock.UtcNow);

            await dbContext.BeginTransaction(default);
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

            await DB.InsertAsync(testRun, dbContext.Session, cancellationToken);
            await DB.InsertAsync(testRunQuestionBatches, dbContext.Session, cancellationToken);
            await DB.SaveOnlyAsync(entity, affectedFields, dbContext.Session, cancellationToken);
            await SendAccessCodes(entity);

            await dbContext.CommitTransaction(cancellationToken);

            return mapper.Map<TestDefinitionViewModel>(entity);
        }

        public async Task<TestDefinitionViewModel> EndTestDefinition(string id, CancellationToken cancellationToken)
        {
            // Verify if status is in running.
            var validEntity = await DB.Find<TestDefinition>()
                .Match(c => c.ID == id && TestDefinition.ActiveStatuses.Contains(c.Status) && c.CurrentTestRun != null)
                .ExecuteSingleAsync(cancellationToken);
            if (validEntity?.CurrentTestRun?.Id is null)
            {
                throw new TestPlatformException("Not Found Activated/Scheduled Test Definition");
            }

            var testRun = await DB.Find<TestRun>().MatchID(validEntity.CurrentTestRun.Id)
                .ExecuteSingleAsync(cancellationToken);
            if (testRun == null)
            {
                throw new TestPlatformException("Not Found Test Run");
            }

            //todo: change to datetime.
            testRun.ExplicitEnd = true;
            testRun.TestDefinitionSnapshot = validEntity;

            var testDefFieldsAffected = validEntity.End();

            await dbContext.BeginTransaction(cancellationToken);
            await DB.SaveOnlyAsync(validEntity, testDefFieldsAffected, dbContext.Session, cancellationToken);
            //todo: improve performance by save affected parts.
            await DB.SaveAsync(testRun, dbContext.Session, cancellationToken);
            await dbContext.CommitTransaction(cancellationToken);

            return mapper.Map<TestDefinitionViewModel>(validEntity);
        }

        public async Task<TestDefinitionViewModel> RestartTestDefinition(string id)
        {
            //todo: concurrency check impl.
            var testDef = await DB.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ??
                          throw new TestPlatformException("NotFoundEntity");

            var affectedFields = testDef.Restart();
            await DB.SaveOnlyAsync(testDef, affectedFields);

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
                var foundEvent =
                    states.FirstOrDefault(c => c.Key.EndsWith(accessCode) && !string.IsNullOrEmpty(c.Value));
                if (foundEvent.Equals(default(BulkStateItem)))
                {
                    continue;
                }

                var parsedEvents =
                    JsonSerializer.Deserialize<TestInvitationEventData>(foundEvent.Value,
                        daprClient.JsonSerializerOptions);
                if (parsedEvents?.Events is null)
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

        public async Task<dynamic> GenerateAccessCodes(string id, int quantity)
        {
            if (quantity <= 0)
            {
                throw new TestPlatformException("Invalid argument quantity");
            }

            // Get and verify the test definition.
            var testDef = await DB.Find<TestDefinition>().MatchID(id)
                .Project(c => c
                    .Include(nameof(TestDefinition.Status))
                    .Include(nameof(TestDefinition.ModifiedOn))
                    .Include(nameof(TestDefinition.CreatedOn))
                    .Include(nameof(TestDefinition.TimeSettings))
                    .Include(nameof(TestDefinition.TestAccessSettings))
                    .Include(nameof(TestDefinition.CurrentTestRun)))
                .ExecuteFirstAsync();
            if (testDef == null)
            {
                throw new EntityNotFoundException(id, nameof(TestDefinition));
            }

            if (testDef.TestAccessSettings.AccessType != TestAcessType.PrivateAccessCode)
            {
                throw new TestPlatformException("Invalid entity");
            }

            if (testDef.LatestStatus == TestDefinitionStatus.Ended)
            {
                throw new TestPlatformException("Invalid status");
            }

            if (testDef.TestAccessSettings.Settings is not PrivateAccessCodeType privateAccessCodeSettings)
            {
                throw new TestPlatformException("Invalid settings");
            }

            for (var i = 0; i < quantity; i++)
            {
                var accessCode = ShortId.Generate(new GenerationOptions(true, false, 10));
                privateAccessCodeSettings.Configs.Add(new PrivateAccessCodeConfig
                {
                    Code = accessCode
                });
            }

            // Update affected property: TestAccessSettings.
            await DB.SaveOnlyAsync(testDef, [nameof(TestDefinition.TestAccessSettings)]);

            return new
            {
                TestAccessSettings = mapper.Map<TestAccessSettingsViewModel>(testDef.TestAccessSettings),
                testDef.ModifiedOn
            };
        }

        public async Task<dynamic> RemoveAccessCodes(string id, string[] codes)
        {
            //todo: reuse logic for 2 same methods.
            // Get and verify the test definition.
            var testDef = await DB
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

            if (testDef.LatestStatus == TestDefinitionStatus.Ended)
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
                {
                    throw new TestPlatformException("Invalid access code");
                }

                privateAccessCodeSettings.Configs.Remove(existingAccessCode);
            }

            // Update affected property: TestAccessSettings.
            await DB.SaveOnlyAsync(testDef, [nameof(TestDefinition.TestAccessSettings)]);

            return new
            {
                TestAccessSettings = mapper.Map<TestAccessSettingsViewModel>(testDef.TestAccessSettings),
                testDef.ModifiedOn
            };
        }

        public async Task SendAccessCodes(string id, string[] codes)
        {
            //todo: reuse logic for 2 same methods.
            // Get and verify the test definition.
            var testDef = await DB.Find<TestDefinition>().MatchID(id).ExecuteFirstAsync() ??
                          throw new EntityNotFoundException(id, nameof(TestDefinition));
            if (testDef.TestAccessSettings.AccessType != TestAcessType.PrivateAccessCode)
            {
                throw new TestPlatformException("Invalid entity");
            }

            if (testDef.LatestStatus == TestDefinitionStatus.Ended)
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
                {
                    throw new TestPlatformException("Invalid access code");
                }

                if (string.IsNullOrWhiteSpace(existingAccessCode.Email))
                {
                    throw new TestPlatformException("Invalid email");
                }

                receivers.Add(new Receiver
                {
                    Email = existingAccessCode.Email,
                    AccessCode = existingAccessCode.Code
                });
            }

            await ProcessSendAccessCodes(testDef, receivers.ToArray());
        }

        private static string[] GetPrivateAccessCodes(TestAccessSettings settings)
        {
            return ((PrivateAccessCodeType)settings).Configs.Select(c => c.Code).ToArray();
        }

        private async Task SendAccessCodes(TestDefinition entity)
        {
            try
            {
                if (entity is { CurrentTestRun: not null, TestAccessSettings.Settings: PrivateAccessCodeType config })
                {
                    var receivers = config.Configs.Where(c => !string.IsNullOrEmpty(c.Email) && c.SendCode)
                        .Select(c => new Receiver(c.Code, c.Email)).ToArray();
                    await ProcessSendAccessCodes(entity, receivers);
                }
            }
            catch (Exception ex)
            {
                logger.LogError("Could not send access code {ex}", ex);
            }
        }

        private async Task ProcessSendAccessCodes(TestDefinition entity, Receiver[] receivers)
        {
            if (receivers.Length == 0)
            {
                return;
            }

            var testRunnerUrl = configuration.GetValue<Uri>("TestRunnerUrl")!;
            var request = new SendTestAccessCodeRequest
            {
                TestUrl = new Uri(testRunnerUrl, $"/start/{entity.ID}").ToString(),
                TestDefinitionId = entity.ID!,
                TestRunId = entity.CurrentTestRun?.Id!,
                Receivers = receivers
            };

            await publishEndpoint.Publish(request);
        }
    }
}