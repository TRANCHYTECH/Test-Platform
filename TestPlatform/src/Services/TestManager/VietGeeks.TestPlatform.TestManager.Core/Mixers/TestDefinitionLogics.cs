using System;
using System.Linq;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core;

public static class TestDefinitionLogics
{
    public static (TestDefinitionStatus Status, DateTime StartAtUtc, DateTime EndAtUtc) EnsureCanActivate(this TestDefinition testDefinition, int questionCount, DateTime checkMoment)
    {
        (TestDefinitionStatus, DateTime, DateTime) result;
        if (testDefinition.TimeSettings == null || testDefinition.TestSetSettings == null || testDefinition.TestAccessSettings == null || testDefinition.GradingSettings == null || testDefinition.TestStartSettings == null)
        {
            throw new TestPlatformException("IncompleteSetup");
        }

        if (testDefinition.TimeSettings?.TestActivationMethod == null)
        {
            throw new TestPlatformException("MissTestActivationMethod");
        }


        if (questionCount <= 0)
        {
            throw new TestPlatformException("MissQuestion");
        }

        if (testDefinition.Status == TestDefinitionStatus.Ended)
        {
            throw new TestPlatformException("TestEnded");
        }

        if (testDefinition.TestInRunning())
        {
            throw new TestPlatformException("TestIsRunning");
        }

        if (testDefinition.TimeSettings.TestActivationMethod is ManualTestActivation manualTestActivation)
        {
            if (manualTestActivation.ActiveUntil == TimeSpan.Zero)
            {
                throw new TestPlatformException("InvalidActiveUntil");
            }

            result = (TestDefinitionStatus.Activated, DateTime.UtcNow, DateTime.UtcNow.Add(manualTestActivation.ActiveUntil));
        }
        else if (testDefinition.TimeSettings.TestActivationMethod is TimePeriodActivation timePeriodActivation)
        {
            if (checkMoment >= timePeriodActivation.ActiveFromDate)
            {
                throw new TestPlatformException("The activate from date is invalid. It's in the pass");
            }

            if (timePeriodActivation.ActiveFromDate >= timePeriodActivation.ActiveUntilDate)
            {
                throw new TestPlatformException("The activate from dates are invalid. It's greater than until date");
            }

            result = (TestDefinitionStatus.Scheduled, timePeriodActivation.ActiveFromDate, timePeriodActivation.ActiveUntilDate);
        }
        else
        {
            throw new TestPlatformException("NotSupportedActivationType");
        }

        return result;
    }

    public static bool TestInRunning(this TestDefinition testDefinition)
    {
        return new[] { TestDefinitionStatus.Activated, TestDefinitionStatus.Scheduled }.Contains(testDefinition.Status)
            && testDefinition.CurrentTestRun != null;
    }
}

