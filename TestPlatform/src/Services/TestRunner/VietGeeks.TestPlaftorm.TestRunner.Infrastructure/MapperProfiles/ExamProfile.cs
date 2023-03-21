using AutoMapper;
using VietGeeks.TestPlatform.TestRunner.Contract.ProctorExamActor;
using Core = VietGeeks.TestPlatform.TestManager.Core.ReadonlyModels;

namespace VietGeeks.TestPlaftorm.TestRunner.Infrastructure.MapperProfiles;

public class ExamProfile : Profile
{
    public ExamProfile()
    {
        CreateMap<Core.AggregatedGrading, AggregatedGrading>();
    }
}