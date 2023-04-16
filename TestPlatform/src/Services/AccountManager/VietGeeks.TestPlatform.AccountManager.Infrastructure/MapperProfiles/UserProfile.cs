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
            CreateMap<User, UserViewModel>();
            CreateMap<UserCreateViewModel, User>()
                .ForMember(c => c.ID, opt => opt.MapFrom(s => s.UserId))
                .ForMember(c => c.Email, opt => opt.MapFrom(s => s.Email));
            CreateMap<RegionalSettings, RegionalSettingsViewModel>();
            CreateMap<RegionalSettingsViewModel, RegionalSettings>();
        }
    }
}

