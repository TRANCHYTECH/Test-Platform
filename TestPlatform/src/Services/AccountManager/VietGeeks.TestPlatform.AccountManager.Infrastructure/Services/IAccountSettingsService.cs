using VietGeeks.TestPlatform.AccountManager.Contract;

namespace VietGeeks.TestPlatform.AccountManager.Infrastructure.Services;

public interface IAccountSettingsService
{
    Task<UserViewModel> CreateUserProfile(UserCreateViewModel viewModel);

    Task<UserViewModel> UpdateUserProfile(UserUpdateViewModel viewModel);

    Task<UserViewModel> GetUserProfile(string userId);

    string[] GetTimeZones();

}