using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Core.Models;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;
using Core = VietGeeks.TestPlatform.TestManager.Core.ReadonlyModels;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.MapperProfiles;

public class ExamProfile : Profile
{
    public ExamProfile()
    {
        CreateMap<Core.AggregatedGrading, AggregatedGradingOuput>();
        CreateMap<Core.PassMarkGrade, PassMarkGradeOutput>();
        CreateMap<StartExamOutput, StartExamOutputViewModel>();
        CreateMap<ExamStatus, ExamStatusWithStep>()
        .ForMember(desc => desc.Step, ops => ops.Ignore());
        CreateMap<TestEndConfig, TestEndConfigOutput>();
        CreateMap<InformRespondentConfig, InformRespondentConfigOutput>();
    }
}