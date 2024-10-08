﻿using FluentValidation.TestHelper;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Infrastructure.Validators.TestDefinition;

namespace VietGeeks.TestPlatform.TestManager.UnitTest;

[Collection(TestDefinitionValidatorTestCollection.CollectionId)]
public class TestAccessSettingsPartValidatorUnitTests(TestDefinitionValidatorFixture fixture)
{
    private readonly TestDefinitionValidatorFixture _fixture = fixture;
    private readonly TestAccessSettingsPartValidator _validator = fixture.CreateTestAccessSettingsPartValidator();

    #region TestAccessSettingsPart

    [Fact]
    public async Task TestAccessSettingsPart_Validate_Failure_MismatchTestAccessType()
    {
        var input = new TestAccessSettingsPart
        {
            AccessType = TestAcessType.PublicLink,
            Settings = new PrivateAccessCodeType
            {
                Configs =
                [
                    new PrivateAccessCodeConfig
                    {
                        Code = "Code001",
                        Email = "dnt@loca.co",
                        SendCode = true,
                        SetId = "set001"
                    }
                ],
                Attempts = 2
            }
        };

        var result = await _validator.TestValidateAsync(input);
        Assert.False(result.IsValid);
        Assert.Equal(1,
            result.Errors.Count(c =>
                c.PropertyName == nameof(TestAccessSettingsPart.Settings) && c.ErrorCode == "PredicateValidator"));
    }

    [Fact]
    public async Task TestAccessSettingsPart_Validate_Success_PublicLinkType()
    {
        var input = new TestAccessSettingsPart
        {
            AccessType = TestAcessType.PublicLink,
            Settings = new PublicLinkType
            {
                RequireAccessCode = true,
                Attempts = 2
            }
        };

        var result = await _validator.TestValidateAsync(input);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task TestAccessSettingsPart_Validate_Success_PrivateAccessCodeType()
    {
        var input = new TestAccessSettingsPart
        {
            AccessType = TestAcessType.PrivateAccessCode,
            Settings = new PrivateAccessCodeType
            {
                Configs =
                [
                    new PrivateAccessCodeConfig
                    {
                        Code = "Code001",
                        Email = "dnt@loca.co",
                        SendCode = true,
                        SetId = "set001"
                    }
                ],
                Attempts = 2
            }
        };

        var result = await _validator.TestValidateAsync(input);
        Assert.True(result.IsValid);
    }

    #endregion
}