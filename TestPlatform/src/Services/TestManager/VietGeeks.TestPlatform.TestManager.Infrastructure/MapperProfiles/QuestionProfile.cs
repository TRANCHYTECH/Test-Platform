using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Core.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.MappingProfiles;

public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<NewQuestionViewModel, QuestionDefinition>();
        CreateMap<QuestionViewModel, QuestionDefinition>().ReverseMap();
        CreateMap<QuestionCategoryViewModel, QuestionCategory>().ReverseMap();
        CreateMap<NewQuestionCategoryViewModel, QuestionCategory>();
        CreateMap<ScoreSettingsViewModel, ScoreSettings>().ReverseMap();
        CreateMap<AnswerViewModel, Answer>().ReverseMap();
    }
}

