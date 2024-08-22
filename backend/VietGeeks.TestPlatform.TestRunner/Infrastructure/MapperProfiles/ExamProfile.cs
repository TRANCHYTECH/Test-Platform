using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Data.Models;
using VietGeeks.TestPlatform.TestManager.Data.ReadonlyModels;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;

namespace VietGeeks.TestPlatform.TestRunner.Infrastructure.MapperProfiles;

public class ExamProfile : Profile
{
    public ExamProfile()
    {
        CreateMap<AggregatedGrading, AggregatedGradingOuput>();
        CreateMap<PassMarkGrade, PassMarkGradeOutput>();
        CreateMap<StartExamOutput, StartExamOutputViewModel>();
        CreateMap<ExamStatus, ExamStatusWithStep>()
            .ForMember(desc => desc.Step, ops => ops.Ignore());
        CreateMap<TestEndConfig, TestEndConfigOutput>();
        CreateMap<InformRespondentConfig, InformRespondentConfigOutput>();
        CreateMap<QuestionDefinition, QuestionOutput>()
            .ForMember(desc => desc.QuestionAnswers, op => op.MapFrom(src => src.Answers))
            .ForMember(desc => desc.TotalPoints,
                op => op.MapFrom(src => src.ScoreSettings != null ? src.ScoreSettings.TotalPoints : 0));
        CreateMap<Answer, AnswerOutput>();
    }
}