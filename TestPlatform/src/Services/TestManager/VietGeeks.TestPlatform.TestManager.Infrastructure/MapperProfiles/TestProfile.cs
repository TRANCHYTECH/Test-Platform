using System;
using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
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
        CreateMap<ITestSetGeneratorViewModel, ITestSetGenerator>()
            .Include<RandomByCategoriesGeneratorViewModel, RandomByCategoriesGenerator>();
        CreateMap<RandomByCategoriesGeneratorViewModel, RandomByCategoriesGenerator>();
        CreateMap<RandomByCategoriesGeneratorConfigViewModel, RandomByCategoriesGeneratorConfig>();
        CreateMap<CreateOrUpdateTestSetSettingsViewModel, TestSetSettingsPart>();

        CreateMap<ITestSetGenerator, ITestSetGeneratorViewModel>()
            .Include<RandomByCategoriesGenerator, RandomByCategoriesGeneratorViewModel>();
        CreateMap<RandomByCategoriesGenerator, RandomByCategoriesGeneratorViewModel>();
        CreateMap<RandomByCategoriesGeneratorConfig, RandomByCategoriesGeneratorConfigViewModel>();
        CreateMap<TestSetSettingsPart, TestSetSettingsViewModel>();
    }
}

