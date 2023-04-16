using System;
using AutoMapper;
using VietGeeks.TestPlatform.AccountManager.Contract;
using VietGeeks.TestPlatform.AccountManager.Core.Models;

namespace VietGeeks.TestPlatform.AccountManager.Infrastructure.MapperProfiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, UserViewModel>()
                .ForMember(c => c.Language, opt => opt.MapFrom(s => s.RegionalSettings.Language))
                .ForMember(c => c.TimeZone, opt => opt.MapFrom(s => s.RegionalSettings.TimeZone));

            CreateMap<UserCreateViewModel, User>()
                .ForMember(c => c.ID, opt => opt.MapFrom(s => s.UserId))
                .ForMember(c => c.Email, opt => opt.MapFrom(s => s.Email));

            CreateMap<RegionalSettingsViewModel, RegionalSettings>();

        }
    }
}

