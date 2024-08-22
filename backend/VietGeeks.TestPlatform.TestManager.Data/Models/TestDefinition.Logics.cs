using MongoDB.Entities;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.TestManager.Data.Models
{
    public partial class TestDefinition
    {
        [Ignore] public TestDefinitionStatus LatestStatus => GetActualStatus(DateTime.UtcNow);

        public static TestDefinitionStatus[] ActiveStatuses =>
        [
            TestDefinitionStatus.Activated, TestDefinitionStatus.Scheduled
        ];

        public TestDefinitionStatus GetActualStatus(DateTime checkMomentUtc)
        {
            if (Status == TestDefinitionStatus.Draft)
            {
                return TestDefinitionStatus.Draft;
            }

            if (Status == TestDefinitionStatus.Ended)
            {
                return TestDefinitionStatus.Ended;
            }

            if (CurrentTestRun == null || TimeSettings == null)
            {
                throw new TestPlatformException("Invalid Test Definition");
            }

            if (ActiveStatuses.Contains(Status))
            {
                var safeCheckMoment = checkMomentUtc.ToUniversalTime();
                if (TimeSettings.TestActivationMethod is ManualTestActivation manualTestActivation)
                {
                    if (safeCheckMoment.ToUniversalTime() >=
                        CurrentTestRun.ActivatedOrScheduledAtUtc.Add(manualTestActivation.ActiveUntil))
                    {
                        return TestDefinitionStatus.Ended;
                    }

                    return TestDefinitionStatus.Activated;
                }

                if (TimeSettings.TestActivationMethod is TimePeriodActivation timePeriodActivationMethod)
                {
                    if (safeCheckMoment < timePeriodActivationMethod.ActiveFromDate)
                    {
                        return TestDefinitionStatus.Scheduled;
                    }

                    if (safeCheckMoment >= timePeriodActivationMethod.ActiveUntilDate)
                    {
                        return TestDefinitionStatus.Ended;
                    }

                    return TestDefinitionStatus.Activated;
                }

                throw new TestPlatformException("Invalid Test Activation Method");
            }

            throw new TestPlatformException("Unknown Status");
        }

        public string[] Activate(string testRunId, DateTime activatedOrScheduledAtUtc, TestDefinitionStatus status)
        {
            if (ActiveStatuses.Contains(status))
            {
                Status = status;
                CurrentTestRun = new CurrentTestRunPart
                {
                    Id = testRunId,
                    // Ensure store utc if user passes local time.
                    ActivatedOrScheduledAtUtc = activatedOrScheduledAtUtc.ToUniversalTime()
                };
            }
            else
            {
                throw new TestPlatformException("Invalid Status Input");
            }

            return [nameof(Status), nameof(CurrentTestRun)];
        }

        public string[] End()
        {
            var actualStatus = LatestStatus;
            if (actualStatus == TestDefinitionStatus.Activated)
            {
                Status = TestDefinitionStatus.Ended;
                CurrentTestRun = null;
            }
            else if (actualStatus == TestDefinitionStatus.Scheduled)
            {
                Status = TestDefinitionStatus.Draft;
                CurrentTestRun = null;
            }
            else
            {
                throw new TestPlatformException("ActionNotAllowed");
            }

            return [nameof(Status), nameof(CurrentTestRun)];
        }

        //todo: verify actual how values are reset.
        public string[] Restart()
        {
            if (LatestStatus != TestDefinitionStatus.Ended)
            {
                throw new TestPlatformException("ActionNotAllowed");
            }

            Status = TestDefinitionStatus.Draft;
            TestAccessSettings = TestAccessSettingsPart.Default();

            return [nameof(Status), nameof(TestAccessSettings)];
        }

        //todo: combine this and above method
        public (TestDefinitionStatus Status, DateTime StartAtUtc, DateTime EndAtUtc) EnsureCanActivate(
            int questionCount, DateTime checkMoment)
        {
            (TestDefinitionStatus, DateTime, DateTime) result;
            if (TimeSettings == null || TestSetSettings == null || TestAccessSettings == null ||
                GradingSettings == null || TestStartSettings == null)
            {
                throw new TestPlatformException("IncompleteSetup");
            }

            if (TimeSettings?.TestActivationMethod == null)
            {
                throw new TestPlatformException("MissTestActivationMethod");
            }


            if (questionCount <= 0)
            {
                throw new TestPlatformException("MissQuestion");
            }

            if (Status == TestDefinitionStatus.Ended)
            {
                throw new TestPlatformException("TestEnded");
            }

            if (TestInRunning())
            {
                throw new TestPlatformException("TestIsRunning");
            }

            if (TimeSettings.TestActivationMethod is ManualTestActivation manualTestActivation)
            {
                if (manualTestActivation.ActiveUntil == TimeSpan.Zero)
                {
                    throw new TestPlatformException("InvalidActiveUntil");
                }

                result = (TestDefinitionStatus.Activated, DateTime.UtcNow,
                    DateTime.UtcNow.Add(manualTestActivation.ActiveUntil));
            }
            else if (TimeSettings.TestActivationMethod is TimePeriodActivation timePeriodActivation)
            {
                if (checkMoment >= timePeriodActivation.ActiveFromDate)
                {
                    throw new TestPlatformException("The activate from date is invalid. It's in the pass");
                }

                if (timePeriodActivation.ActiveFromDate >= timePeriodActivation.ActiveUntilDate)
                {
                    throw new TestPlatformException(
                        "The activate from dates are invalid. It's greater than until date");
                }

                result = (TestDefinitionStatus.Scheduled, timePeriodActivation.ActiveFromDate,
                    timePeriodActivation.ActiveUntilDate);
            }
            else
            {
                throw new TestPlatformException("NotSupportedActivationType");
            }

            return result;
        }

        public bool TestInRunning()
        {
            return ActiveStatuses.Contains(Status) && CurrentTestRun != null;
        }
    }
}