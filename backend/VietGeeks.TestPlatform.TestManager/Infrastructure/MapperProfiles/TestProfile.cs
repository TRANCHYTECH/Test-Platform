using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Contract.ViewModels;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using AnswerQuestionConfig = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.AnswerQuestionConfig;
using CompleteQuestionDuration = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.CompleteQuestionDuration;
using CompleteTestDuration = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.CompleteTestDuration;
using GradeRangeCriteria = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.GradeRangeCriteria;
using GradeRangeCriteriaDetail = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.GradeRangeCriteriaDetail;
using GradingCriteriaConfig = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.GradingCriteriaConfig;
using InformRespondentConfig = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.InformRespondentConfig;
using ManualTestActivation = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.ManualTestActivation;
using PassMaskCriteria = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.PassMaskCriteria;
using RespondentIdentifyConfig = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.RespondentIdentifyConfig;
using TestActivationMethod = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.TestActivationMethod;
using TestDurationMethod = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.TestDurationMethod;
using TestEndConfig = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.TestEndConfig;
using TimePeriodActivation = VietGeeks.TestPlatform.TestManager.Contract.ViewModels.TimePeriodActivation;

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

        CreateMap<TestDefinition, TestDefinitionViewModel>()
            .ForMember(d => d.Status, opt => opt.MapFrom(s => s.LatestStatus));
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

        CreateMap<GradingCriteriaConfig, Data.Models.GradingCriteriaConfig>().IncludeAllDerived();
        CreateMap<PassMaskCriteria, Data.Models.PassMaskCriteria>();
        CreateMap<GradeRangeCriteria, Data.Models.GradeRangeCriteria>();
        CreateMap<GradeRangeCriteriaDetail, Data.Models.GradeRangeCriteriaDetail>();
        CreateMap<TestEndConfig, Data.Models.TestEndConfig>();
        CreateMap<InformRespondentConfig, Data.Models.InformRespondentConfig>();
        CreateMap<CreateOrUpdateGradingSettings, GradingSettingsPart>();

        CreateMap<Data.Models.GradingCriteriaConfig, GradingCriteriaConfig>().IncludeAllDerived();
        CreateMap<Data.Models.PassMaskCriteria, PassMaskCriteria>();
        CreateMap<Data.Models.GradeRangeCriteria, GradeRangeCriteria>();
        CreateMap<Data.Models.GradeRangeCriteriaDetail, GradeRangeCriteriaDetail>();
        CreateMap<Data.Models.TestEndConfig, TestEndConfig>();
        CreateMap<Data.Models.InformRespondentConfig, InformRespondentConfig>();
        CreateMap<GradingSettingsPart, GradingSettings>();

        CreateMap<TestActivationMethod, Data.Models.TestActivationMethod>().IncludeAllDerived();
        CreateMap<ManualTestActivation, Data.Models.ManualTestActivation>();
        CreateMap<TimePeriodActivation, Data.Models.TimePeriodActivation>();
        CreateMap<TestDurationMethod, Data.Models.TestDurationMethod>().IncludeAllDerived();
        CreateMap<CompleteQuestionDuration, Data.Models.CompleteQuestionDuration>();
        CreateMap<CompleteTestDuration, Data.Models.CompleteTestDuration>();
        CreateMap<AnswerQuestionConfig, Data.Models.AnswerQuestionConfig>();
        CreateMap<CreateOrUpdateTimeSettingsViewModel, TimeSettingsPart>();

        CreateMap<Data.Models.TestActivationMethod, TestActivationMethod>().IncludeAllDerived();
        CreateMap<Data.Models.ManualTestActivation, ManualTestActivation>();
        CreateMap<Data.Models.TimePeriodActivation, TimePeriodActivation>();
        CreateMap<Data.Models.TestDurationMethod, TestDurationMethod>().IncludeAllDerived();
        CreateMap<Data.Models.CompleteQuestionDuration, CompleteQuestionDuration>();
        CreateMap<Data.Models.CompleteTestDuration, CompleteTestDuration>();
        CreateMap<Data.Models.AnswerQuestionConfig, AnswerQuestionConfig>();
        CreateMap<TimeSettingsPart, TimeSettings>();
        CreateMap<CurrentTestRunPart, CurrentTestRun>();

        CreateMap<CreateOrUpdateTestStartSettingsViewModel, TestStartSettingsPart>();
        CreateMap<RespondentIdentifyConfig, Data.Models.RespondentIdentifyConfig>();

        CreateMap<TestStartSettingsPart, TestStartSettingsViewModel>();
        CreateMap<Data.Models.RespondentIdentifyConfig, RespondentIdentifyConfig>();
    }
}