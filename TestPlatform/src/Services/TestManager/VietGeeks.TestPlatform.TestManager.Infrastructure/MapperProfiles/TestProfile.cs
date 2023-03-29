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
        CreateMap<TestDefinition, TestDefinitionOverview>()
            .ForMember(d => d.Id, opt => opt.MapFrom(s => s.ID))
            .ForMember(d => d.Name, opt => opt.MapFrom(s => s.BasicSettings.Name))
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.BasicSettings.Description))
            .ForMember(d => d.CreatedOn, opt => opt.MapFrom(s => s.CreatedOn))
            .ForMember(d => d.Category, opt => opt.MapFrom(s => s.BasicSettings.Category))
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.LatestStatus));

        CreateMap<NewTestDefinitionViewModel, TestDefinition>();
        CreateMap<CreateOrUpdateTestBasicSettingsViewModel, TestBasicSettingsPart>();

        CreateMap<TestDefinition, TestDefinitionViewModel>().ForMember(d => d.Status, opt => opt.MapFrom(s => s.LatestStatus));
        CreateMap<TestBasicSettingsPart, TestBasicSettingsViewModel>();

        CreateMap<NewTestCategoryViewModel, TestCategory>();
        CreateMap<TestCategory, TestCategoryViewModel>();

        // Test set settings mapping.
        CreateMap<TestSetGeneratorViewModel, TestSetGenerator>()
            .Include<DefaultGeneratorViewModel, DefaultGenerator>()
            .Include<RandomByCategoriesGeneratorViewModel, RandomFromCategoriesGenerator>();
        CreateMap<DefaultGeneratorViewModel, DefaultGenerator>();
        CreateMap<RandomByCategoriesGeneratorViewModel, RandomFromCategoriesGenerator>();
        CreateMap<RandomByCategoriesGeneratorConfigViewModel, RandomFromCategoriesGeneratorConfig>();
        CreateMap<CreateOrUpdateTestSetSettingsViewModel, TestSetSettingsPart>();

        CreateMap<TestSetGenerator, TestSetGeneratorViewModel>()
            .Include<DefaultGenerator, DefaultGeneratorViewModel>()
            .Include<RandomFromCategoriesGenerator, RandomByCategoriesGeneratorViewModel>();
        CreateMap<DefaultGenerator, DefaultGeneratorViewModel>();
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
        CreateMap<Contract.ViewModels.InformRespondentConfig, Core.Models.InformRespondentConfig>();
        CreateMap<Contract.ViewModels.CreateOrUpdateGradingSettings, Core.Models.GradingSettingsPart>();

        CreateMap<Core.Models.GradingCriteriaConfig, Contract.ViewModels.GradingCriteriaConfig>().IncludeAllDerived();
        CreateMap<Core.Models.PassMaskCriteria, Contract.ViewModels.PassMaskCriteria>();
        CreateMap<Core.Models.GradeRangeCriteria, Contract.ViewModels.GradeRangeCriteria>();
        CreateMap<Core.Models.GradeRangeCriteriaDetail, Contract.ViewModels.GradeRangeCriteriaDetail>();
        CreateMap<Core.Models.TestEndConfig, Contract.ViewModels.TestEndConfig>();
        CreateMap<Core.Models.InformRespondentConfig, Contract.ViewModels.InformRespondentConfig >();
        CreateMap<Core.Models.GradingSettingsPart, Contract.ViewModels.GradingSettings>();

        CreateMap<Contract.ViewModels.TestActivationMethod, Core.Models.TestActivationMethod>().IncludeAllDerived();
        CreateMap<Contract.ViewModels.ManualTestActivation, Core.Models.ManualTestActivation>();
        CreateMap<Contract.ViewModels.TimePeriodActivation, Core.Models.TimePeriodActivation>();
        CreateMap<Contract.ViewModels.TestDurationMethod, Core.Models.TestDurationMethod>().IncludeAllDerived();
        CreateMap<Contract.ViewModels.CompleteQuestionDuration, Core.Models.CompleteQuestionDuration>();
        CreateMap<Contract.ViewModels.CompleteTestDuration, Core.Models.CompleteTestDuration>();
        CreateMap<Contract.ViewModels.AnswerQuestionConfig, Core.Models.AnswerQuestionConfig>();
        CreateMap<Contract.ViewModels.CreateOrUpdateTimeSettingsViewModel, Core.Models.TimeSettingsPart>();

        CreateMap<Core.Models.TestActivationMethod, Contract.ViewModels.TestActivationMethod>().IncludeAllDerived();
        CreateMap<Core.Models.ManualTestActivation, Contract.ViewModels.ManualTestActivation>();
        CreateMap<Core.Models.TimePeriodActivation, Contract.ViewModels.TimePeriodActivation>();
        CreateMap<Core.Models.TestDurationMethod, Contract.ViewModels.TestDurationMethod>().IncludeAllDerived();
        CreateMap<Core.Models.CompleteQuestionDuration, Contract.ViewModels.CompleteQuestionDuration>();
        CreateMap<Core.Models.CompleteTestDuration, Contract.ViewModels.CompleteTestDuration>();
        CreateMap<Core.Models.AnswerQuestionConfig, Contract.ViewModels.AnswerQuestionConfig>();
        CreateMap<Core.Models.TimeSettingsPart, Contract.ViewModels.TimeSettings>();
        CreateMap<Core.Models.CurrentTestRunPart, Contract.ViewModels.CurrentTestRun>();

        CreateMap<Contract.ViewModels.CreateOrUpdateTestStartSettingsViewModel, Core.Models.TestStartSettingsPart>();
        CreateMap<Contract.ViewModels.RespondentIdentifyConfig, Core.Models.RespondentIdentifyConfig>();

        CreateMap<Core.Models.TestStartSettingsPart, Contract.ViewModels.TestStartSettingsViewModel>();
        CreateMap<Core.Models.RespondentIdentifyConfig, Contract.ViewModels.RespondentIdentifyConfig>();

    }
}

