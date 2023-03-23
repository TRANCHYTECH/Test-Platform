using System;
using System.Linq;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Core;

public static class TestDefinitionLogics
{
    public static (DateTime StartAtUtc, DateTime EndAtUtc) EnsureCanActivate(this TestDefinition testDefinition, int questionCount)
    {
        if (testDefinition.Status == TestDefinitionStatus.Ended)
        {
            throw new TestPlatformException("TestEnded");
        }

        if (testDefinition.TimeSettings == null || testDefinition.TestSetSettings == null || testDefinition.TestAccessSettings == null || testDefinition.GradingSettings == null || testDefinition.TestStartSettings == null)
        {
            throw new TestPlatformException("IncompleteSetup");
        }

        if (testDefinition.TimeSettings?.TestActivationMethod == null)
        {
            throw new TestPlatformException("MissTestActivationMethod");
        }

        if ((testDefinition.TimeSettings.TestActivationMethod is ManualTestActivation method) == false)
        {
            throw new TestPlatformException("WrongActivationMethod");
        }

        if (questionCount <= 0)
        {
            throw new TestPlatformException("MissQuestion");
        }

        return (DateTime.UtcNow, DateTime.UtcNow.Add(method.ActiveUntil));
    }

    public static void EnsureCanSchedule(this TestDefinition testDefinition, DateTime scheduledMoment)
    {
        if (testDefinition.Status == TestDefinitionStatus.Ended)
        {
            throw new TestPlatformException("TestEnded");

        }

        if (testDefinition.TimeSettings == null || testDefinition.TestSetSettings == null || testDefinition.TestAccessSettings == null || testDefinition.GradingSettings == null || testDefinition.TestStartSettings == null)
        {
            throw new TestPlatformException("IncompleteSetup");
        }

        if (testDefinition.TimeSettings?.TestActivationMethod == null)
        {
            throw new TestPlatformException("MissTestActivationMethod");
        }

        if ((testDefinition.TimeSettings.TestActivationMethod is TimePeriodActivation method) == false)
        {
            throw new TestPlatformException("WrongActivationMethod");
        }

        if (scheduledMoment > method.ActiveFromDate)
        {
            throw new TestPlatformException("InvalidActiveFromDate");
        }
    }

    public static bool TestInRunning(this TestDefinition testDefinition)
    {
        return new[] { TestDefinitionStatus.Activated, TestDefinitionStatus.Scheduled }.Contains(testDefinition.Status)
            && testDefinition.CurrentTestRun != null;
    }
}

