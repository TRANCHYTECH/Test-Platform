using AutoMapper;
using VietGeeks.TestPlatform.TestManager.Contract;
using VietGeeks.TestPlatform.TestManager.Data.Models;

namespace VietGeeks.TestPlatform.TestManager.Infrastructure.MapperProfiles;

public class QuestionProfile : Profile
{
    public QuestionProfile()
    {
        CreateMap<CreateOrUpdateQuestionViewModel, QuestionDefinition>();
        CreateMap<QuestionViewModel, QuestionDefinition>().ReverseMap();
        CreateMap<QuestionCategoryViewModel, QuestionCategory>().ReverseMap();
        CreateMap<NewQuestionCategoryViewModel, QuestionCategory>();
        CreateMap<AnswerViewModel, Answer>().ReverseMap();

        CreateMap<ScoreSettingsViewModel, ScoreSettings>().IncludeAllDerived();
        CreateMap<ScoreSettings,ScoreSettingsViewModel>().IncludeAllDerived();
        CreateMap<SingleChoiceScoreSettingsViewModel, SingleChoiceScoreSettings>().ReverseMap();
        CreateMap<MultipleChoiceScoreSettingsViewModel, MultipleChoiceScoreSettings>().ReverseMap();
    }
}