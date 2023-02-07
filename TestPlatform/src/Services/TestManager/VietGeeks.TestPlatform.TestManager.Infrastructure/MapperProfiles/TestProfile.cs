using System;
using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.MappingProfiles;

public class TestProfile : Profile
{
    public TestProfile()
    {
        CreateMap<NewTestDefinitionViewModel, TestDefinition>();
        CreateMap<CreateOrUpdateTestBasicSettingsViewModel, TestBasicSettingsPart>();

        CreateMap<TestDefinition, TestDefinitionViewModel>();
        CreateMap<TestBasicSettingsPart, TestBasicSettingsViewModel>();

        CreateMap<NewTestCategoryViewModel, TestCategory>();
        CreateMap<TestCategory, TestCategoryViewModel>();

        // Test set settings mapping.
        CreateMap<TestSetGeneratorViewModel, TestSetGenerator>()
            .Include<RandomByCategoriesGeneratorViewModel, RandomFromCategoriesGenerator>();
        CreateMap<RandomByCategoriesGeneratorViewModel, RandomFromCategoriesGenerator>();
        CreateMap<RandomByCategoriesGeneratorConfigViewModel, RandomFromCategoriesGeneratorConfig>();
        CreateMap<CreateOrUpdateTestSetSettingsViewModel, TestSetSettingsPart>();

        CreateMap<TestSetGenerator, TestSetGeneratorViewModel>()
            .Include<RandomFromCategoriesGenerator, RandomByCategoriesGeneratorViewModel>();
        CreateMap<RandomFromCategoriesGenerator, RandomByCategoriesGeneratorViewModel>();
        CreateMap<RandomFromCategoriesGeneratorConfig, RandomByCategoriesGeneratorConfigViewModel>();
        CreateMap<TestSetSettingsPart, TestSetSettingsViewModel>();

        //
        CreateMap<TestAccessTypeViewModel, TestAccessType>()
            .Include<PublicLinkTypeViewModel, PublicLinkType>();
        CreateMap<PublicLinkTypeViewModel, PublicLinkType>();
        CreateMap<PrivateAccessCodeTypeViewModel, PrivateAccessCodeType>();
        CreateMap<PrivateAccessCodeConfigViewModel, PrivateAccessCodeConfig>();
        CreateMap<CreateOrUpdateTestAccessSettingsViewModel, TestAccessSettingsPart>();

        CreateMap<TestAccessType, TestAccessTypeViewModel>()
            .Include<PublicLinkType, PublicLinkTypeViewModel>();
        CreateMap<PublicLinkType, PublicLinkTypeViewModel>();
        CreateMap<PrivateAccessCodeType, PrivateAccessCodeTypeViewModel>();
        CreateMap<PrivateAccessCodeConfig, PrivateAccessCodeConfigViewModel>();
        CreateMap<TestAccessSettingsPart, TestAccessSettingsViewModel>();

    }
}

