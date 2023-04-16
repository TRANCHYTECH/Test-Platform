using System;
using AutoMapper;
using MongoDB.Entities;
using NodaTime;
using NodaTime.TimeZones;
using VietGeeks.TestPlatform.AccountManager.Core.Models;
using VietGeeks.TestPlatform.AccountManager.Contract;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.AccountManager.Infrastructure.Services
{
    public class AccountSettingsService : IAccountSettingsService
    {
        private readonly IMapper _mapper;

        public AccountSettingsService(IMapper mapper)
        {
            _mapper = mapper;
        }

        public async Task<UserViewModel> CreateUserProfile(UserCreateViewModel viewModel)
        {
            var userProfile = await DB.Find<User>().Match(c => c.ID == viewModel.UserId || c.Email == viewModel.Email).ExecuteSingleAsync();
            if(userProfile != null)
                throw new TestPlatformException("User with Id or Email already exists");

            var newUser = _mapper.Map<User>(viewModel);

            //todo: use distributed lock here
            await DB.InsertAsync(newUser);

            return _mapper.Map<UserViewModel>(userProfile);
        }

        public async Task<UserViewModel> UpdateUserProfile(UserUpdateViewModel viewModel)
        {
            var userProfile = await DB.Find<User>().MatchID(viewModel.UserId).ExecuteSingleAsync();
            if (userProfile == null)
                throw new TestPlatformException("Not found user");

            var affectedProperties = new List<string>();
            if(viewModel.RegionalSettings != null)
            {
                userProfile.RegionalSettings = _mapper.Map<RegionalSettings>(viewModel.RegionalSettings);

                //todo: validate timezone, langugage are valid.
                affectedProperties.Add(nameof(User.RegionalSettings));
            }

            if (affectedProperties.Count > 0)
            {
                await DB.SaveOnlyAsync(userProfile, affectedProperties);
            }

            return _mapper.Map<UserViewModel>(userProfile);
        }

        public async Task<UserViewModel> GetUserProfile(string userId)
        {
            var userProfile = await DB.Find<User>().MatchID(userId).ExecuteSingleAsync();

            return _mapper.Map<UserViewModel>(userProfile);
        }

        public string[] GetTimeZones()
        {
            return DateTimeZoneProviders.Tzdb.Ids.ToArray();
        }
    }
}