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
        CreateMap<TestAccessConfigViewModel, TestAccessSettings>().IncludeAllDerived();
        CreateMap<PublicLinkTypeViewModel, PublicLinkType>();
        CreateMap<PrivateAccessCodeTypeViewModel, PrivateAccessCodeType>();
        CreateMap<PrivateAccessCodeConfigViewModel, PrivateAccessCodeConfig>();
        CreateMap<GroupPasswordTypeViewModel, GroupPasswordType>();
        CreateMap<TrainingTypeViewModel, TrainingType>();
        CreateMap<CreateOrUpdateTestAccessSettingsViewModel, TestAccessSettingsPart>();

        CreateMap<TestAccessSettings, TestAccessConfigViewModel>().IncludeAllDerived();
        CreateMap<PublicLinkType, PublicLinkTypeViewModel>();
        CreateMap<PrivateAccessCodeType, PrivateAccessCodeTypeViewModel>();
        CreateMap<PrivateAccessCodeConfig, PrivateAccessCodeConfigViewModel>();
        CreateMap<GroupPasswordType, GroupPasswordTypeViewModel>();
        CreateMap<TrainingType, TrainingTypeViewModel>();
        CreateMap<TestAccessSettingsPart, TestAccessSettingsViewModel>();

        CreateMap<Contract.ViewModels.GradingCriteriaConfig, Core.Models.GradingCriteriaConfig>().IncludeAllDerived();
        CreateMap<Contract.ViewModels.PassMaskCriteria, Core.Models.PassMaskCriteria>();
        CreateMap<Contract.ViewModels.GradeRangeCriteria, Core.Models.GradeRangeCriteria>();
        CreateMap<Contract.ViewModels.GradeRangeCriteriaDetail, Core.Models.GradeRangeCriteriaDetail>();
        CreateMap<Contract.ViewModels.TestEndConfig, Core.Models.TestEndConfig>();
        CreateMap<Contract.ViewModels.CreateOrupdateGradingSettings, Core.Models.GradingSettingsPart>();

        CreateMap<Core.Models.GradingCriteriaConfig, Contract.ViewModels.GradingCriteriaConfig>().IncludeAllDerived();
        CreateMap<Core.Models.PassMaskCriteria, Contract.ViewModels.PassMaskCriteria>();
        CreateMap<Core.Models.GradeRangeCriteria, Contract.ViewModels.GradeRangeCriteria>();
        CreateMap<Core.Models.GradeRangeCriteriaDetail, Contract.ViewModels.GradeRangeCriteriaDetail>();
        CreateMap<Core.Models.TestEndConfig, Contract.ViewModels.TestEndConfig>();
        CreateMap<Core.Models.GradingSettingsPart, Contract.ViewModels.GradingSettings>();
    }
}

