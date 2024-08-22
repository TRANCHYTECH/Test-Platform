using AutoMapper;
using MongoDB.Entities;
using NodaTime;
using VietGeeks.TestPlatform.AccountManager.Contract;
using VietGeeks.TestPlatform.AccountManager.Data;
using VietGeeks.TestPlatform.SharedKernel.Exceptions;

namespace VietGeeks.TestPlatform.AccountManager.Services;

public class AccountSettingsService(IMapper mapper) : IAccountSettingsService
{
    public async Task<UserViewModel> CreateUserProfile(UserCreateViewModel viewModel)
    {
        var userProfile = await DB.Find<User>().Match(c => c.ID == viewModel.UserId || c.Email == viewModel.Email)
            .ExecuteSingleAsync();
        if (userProfile != null)
        {
            throw new TestPlatformException("User with Id or Email already exists");
        }

        var newUser = mapper.Map<User>(viewModel);

        //todo: use distributed lock here
        await DB.InsertAsync(newUser);

        return mapper.Map<UserViewModel>(userProfile);
    }

    public async Task<UserViewModel> UpdateUserProfile(UserUpdateViewModel viewModel)
    {
        var userProfile = await DB.Find<User>().MatchID(viewModel.UserId).ExecuteSingleAsync();
        if (userProfile == null)
        {
            throw new TestPlatformException("Not found user");
        }

        var affectedProperties = new List<string>();
        if (viewModel.RegionalSettings != null)
        {
            userProfile.RegionalSettings = mapper.Map<RegionalSettings>(viewModel.RegionalSettings);

            //todo: validate timezone, langugage are valid.
            affectedProperties.Add(nameof(User.RegionalSettings));
        }

        if (affectedProperties.Count > 0)
        {
            await DB.SaveOnlyAsync(userProfile, affectedProperties);
        }

        return mapper.Map<UserViewModel>(userProfile);
    }

    public async Task<UserViewModel> GetUserProfile(string userId)
    {
        var userProfile = await DB.Find<User>().MatchID(userId).ExecuteSingleAsync();

        return mapper.Map<UserViewModel>(userProfile);
    }

    public string[] GetTimeZones()
    {
        return DateTimeZoneProviders.Tzdb.Ids.ToArray();
    }
}