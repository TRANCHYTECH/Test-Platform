using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition;

namespace VietGeeks.TestPlatform.TestManager.UnitTest
{
    [Collection(TestDefinitionValidatorTestCollection.CollectionId)]
    public class TimeSettingsPartValidatorUnitTests(TestDefinitionValidatorFixture fixture)
    {
        private readonly TestDefinitionValidatorFixture _fixture = fixture;
        private readonly TimeSettingsPartValidator _validator = fixture.CreateTimeSettingsPartValidator();

        [Fact]
        public async Task TimeSettingsPart_AnswerQuestionConfig_Validate_Failure()
        {
            var testedPart = TimeSettingsPart.Default();
            testedPart.AnswerQuestionConfig.SkipQuestion = true;

            var result = await _validator.ValidateAsync(testedPart);

            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count(c => c.PropertyName == "AnswerQuestionConfig.SkipQuestion"));
        }

        [Theory]
        [MemberData(nameof(GetTestData_AnswerQuestionConfig_Success))]
        public async Task TimeSettingsPart_AnswerQuestionConfig_Validate_Success(TimeSettingsPart testedPart)
        {
            var result = await _validator.ValidateAsync(testedPart);

            Assert.True(result.IsValid);
        }

        [Theory]
        [MemberData(nameof(GetTestData_TestDurationMethod_Failure))]
        public async Task TimeSettingsPart_TestDurationMethod_Validate_Failure(TestDurationMethod input)
        {
            var testedPart = TimeSettingsPart.Default();
            testedPart.TestDurationMethod = input;

            var result = await _validator.ValidateAsync(testedPart);

            Assert.False(result.IsValid);
            Assert.Equal(1, result.Errors.Count(c => c.PropertyName == "TestDurationMethod.Duration"));
        }

        [Theory]
        [MemberData(nameof(GetTestData_TestActivationMethod_Failure))]
        public async Task TimeSettingsPart_TestActivationMethod_Validate_Failure(TestActivationMethod input)
        {
            var testedPart = TimeSettingsPart.Default();
            testedPart.TestActivationMethod = input;

            var result = await _validator.ValidateAsync(testedPart);

            Assert.False(result.IsValid);
            Assert.Equal(1,
                result.Errors.Count(c => c.PropertyName.StartsWith(nameof(TimeSettingsPart.TestActivationMethod))));
        }

        [Theory]
        [MemberData(nameof(GetTestData_TestDurationMethod_Success))]
        public async Task TimeSettingsPart_TestDurationMethod_Validate_Success(TestDurationMethod input)
        {
            var testedPart = TimeSettingsPart.Default();
            testedPart.TestDurationMethod = input;

            var result = await _validator.ValidateAsync(testedPart);

            Assert.True(result.IsValid);
        }

        [Theory]
        [MemberData(nameof(GetTestData_TestActivationMethod_Success))]
        public async Task TimeSettingsPart_TestActivationMethod_Validate_Success(TestActivationMethod input)
        {
            var testedPart = TimeSettingsPart.Default();
            testedPart.TestActivationMethod = input;

            var result = await _validator.ValidateAsync(testedPart);

            Assert.True(result.IsValid);
        }

        public static IEnumerable<object[]> GetTestData_TestActivationMethod_Failure()
        {
            var wrong1 = new ManualTestActivation
            {
                ActiveUntil = TimeSpan.Parse("00:00:00")
            };

            var wrong2 = new ManualTestActivation
            {
                ActiveUntil = TimeSpan.FromDays(100)
            };

            var wrong3 = new TimePeriodActivation
            {
                ActiveFromDate = DateTime.UtcNow.AddMinutes(-10),
                ActiveUntilDate = DateTime.UtcNow.AddMinutes(-5)
            };

            var wrong4 = new TimePeriodActivation
            {
                ActiveFromDate = DateTime.UtcNow.AddMinutes(2),
                ActiveUntilDate = DateTime.UtcNow.AddMinutes(1)
            };

            return new List<object[]>
            {
                new object[] { wrong1 },
                new object[] { wrong2 },
                new object[] { wrong3 },
                new object[] { wrong4 }
            };
        }

        public static IEnumerable<object[]> GetTestData_TestDurationMethod_Failure()
        {
            var wrong0 = new CompleteQuestionDuration();

            var wrong1 = new CompleteQuestionDuration
            {
                Duration = TimeSpan.Parse("00:00:00")
            };

            var wrong2 = new CompleteQuestionDuration
            {
                Duration = TimeSpan.Parse("01:00:00")
            };

            var wrong3 = new CompleteTestDuration
            {
                Duration = TimeSpan.Parse("00:00:00")
            };

            var wrong4 = new CompleteTestDuration
            {
                Duration = TimeSpan.Parse("1.00:00:00")
            };

            return new List<object[]>
            {
                new object[] { wrong0 },
                new object[] { wrong1 },
                new object[] { wrong2 },
                new object[] { wrong3 },
                new object[] { wrong4 }
            };
        }

        public static IEnumerable<object[]> GetTestData_AnswerQuestionConfig_Success()
        {
            var right1 = TimeSettingsPart.Default();
            right1.AnswerQuestionConfig.SkipQuestion = false;

            var right2 = TimeSettingsPart.Default();
            right2.TestDurationMethod = new CompleteTestDuration { Duration = TimeSpan.FromHours(12) };
            right1.AnswerQuestionConfig.SkipQuestion = false;

            return new List<object[]>
            {
                new object[] { right1 },
                new object[] { right2 }
            };
        }

        public static IEnumerable<object[]> GetTestData_TestDurationMethod_Success()
        {
            var wrong1 = new CompleteQuestionDuration
            {
                Duration = TimeSpan.Parse("00:00:01")
            };

            var wrong2 = new CompleteQuestionDuration
            {
                Duration = TimeSpan.Parse("00:59:59")
            };

            var wrong3 = new CompleteTestDuration
            {
                Duration = TimeSpan.Parse("00:00:01")
            };

            var wrong4 = new CompleteTestDuration
            {
                Duration = TimeSpan.Parse("23:59:59")
            };

            return new List<object[]>
            {
                new object[] { wrong1 },
                new object[] { wrong2 },
                new object[] { wrong3 },
                new object[] { wrong4 }
            };
        }

        public static IEnumerable<object[]> GetTestData_TestActivationMethod_Success()
        {
            var right1 = new ManualTestActivation
            {
                ActiveUntil = TimeSpan.Parse("00:00:01")
            };

            var right2 = new ManualTestActivation
            {
                ActiveUntil = TimeSpan.Parse("99.23:59:59")
            };

            var right3 = new TimePeriodActivation
            {
                ActiveFromDate = DateTime.UtcNow.AddSeconds(10),
                ActiveUntilDate = DateTime.UtcNow.AddMinutes(1)
            };


            return new List<object[]>
            {
                new object[] { right1 },
                new object[] { right2 },
                new object[] { right3 }
            };
        }
    }
}