using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.MapperProfiles;

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

        CreateMap<Contract.ViewModels.GradingCriteriaConfig, Data.Models.GradingCriteriaConfig>().IncludeAllDerived();
        CreateMap<Contract.ViewModels.PassMaskCriteria, Data.Models.PassMaskCriteria>();
        CreateMap<Contract.ViewModels.GradeRangeCriteria, Data.Models.GradeRangeCriteria>();
        CreateMap<Contract.ViewModels.GradeRangeCriteriaDetail, Data.Models.GradeRangeCriteriaDetail>();
        CreateMap<Contract.ViewModels.TestEndConfig, Data.Models.TestEndConfig>();
        CreateMap<Contract.ViewModels.InformRespondentConfig, Data.Models.InformRespondentConfig>();
        CreateMap<Contract.ViewModels.CreateOrUpdateGradingSettings, GradingSettingsPart>();

        CreateMap<Data.Models.GradingCriteriaConfig, Contract.ViewModels.GradingCriteriaConfig>().IncludeAllDerived();
        CreateMap<Data.Models.PassMaskCriteria, Contract.ViewModels.PassMaskCriteria>();
        CreateMap<Data.Models.GradeRangeCriteria, Contract.ViewModels.GradeRangeCriteria>();
        CreateMap<Data.Models.GradeRangeCriteriaDetail, Contract.ViewModels.GradeRangeCriteriaDetail>();
        CreateMap<Data.Models.TestEndConfig, Contract.ViewModels.TestEndConfig>();
        CreateMap<Data.Models.InformRespondentConfig, Contract.ViewModels.InformRespondentConfig >();
        CreateMap<GradingSettingsPart, Contract.ViewModels.GradingSettings>();

        CreateMap<Contract.ViewModels.TestActivationMethod, Data.Models.TestActivationMethod>().IncludeAllDerived();
        CreateMap<Contract.ViewModels.ManualTestActivation, Data.Models.ManualTestActivation>();
        CreateMap<Contract.ViewModels.TimePeriodActivation, Data.Models.TimePeriodActivation>();
        CreateMap<Contract.ViewModels.TestDurationMethod, Data.Models.TestDurationMethod>().IncludeAllDerived();
        CreateMap<Contract.ViewModels.CompleteQuestionDuration, Data.Models.CompleteQuestionDuration>();
        CreateMap<Contract.ViewModels.CompleteTestDuration, Data.Models.CompleteTestDuration>();
        CreateMap<Contract.ViewModels.AnswerQuestionConfig, Data.Models.AnswerQuestionConfig>();
        CreateMap<Contract.ViewModels.CreateOrUpdateTimeSettingsViewModel, TimeSettingsPart>();

        CreateMap<Data.Models.TestActivationMethod, Contract.ViewModels.TestActivationMethod>().IncludeAllDerived();
        CreateMap<Data.Models.ManualTestActivation, Contract.ViewModels.ManualTestActivation>();
        CreateMap<Data.Models.TimePeriodActivation, Contract.ViewModels.TimePeriodActivation>();
        CreateMap<Data.Models.TestDurationMethod, Contract.ViewModels.TestDurationMethod>().IncludeAllDerived();
        CreateMap<Data.Models.CompleteQuestionDuration, Contract.ViewModels.CompleteQuestionDuration>();
        CreateMap<Data.Models.CompleteTestDuration, Contract.ViewModels.CompleteTestDuration>();
        CreateMap<Data.Models.AnswerQuestionConfig, Contract.ViewModels.AnswerQuestionConfig>();
        CreateMap<TimeSettingsPart, Contract.ViewModels.TimeSettings>();
        CreateMap<CurrentTestRunPart, Contract.ViewModels.CurrentTestRun>();

        CreateMap<Contract.ViewModels.CreateOrUpdateTestStartSettingsViewModel, TestStartSettingsPart>();
        CreateMap<Contract.ViewModels.RespondentIdentifyConfig, Data.Models.RespondentIdentifyConfig>();

        CreateMap<TestStartSettingsPart, Contract.ViewModels.TestStartSettingsViewModel>();
        CreateMap<Data.Models.RespondentIdentifyConfig, Contract.ViewModels.RespondentIdentifyConfig>();

    }
}

