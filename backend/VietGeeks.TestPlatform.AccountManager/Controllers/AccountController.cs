using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using VietGeeks.TestPlatform.AccountManager.Contract;
using VietGeeks.TestPlatform.AccountManager.Services;
using VietGeeks.TestPlatform.AspNetCore;
using VietGeeks.TestPlatform.AspNetCore.Services;

namespace VietGeeks.TestPlatform.AccountManager.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class AccountController(
    ILogger<AccountController> logger,
    IAccountSettingsService accountSettingsService,
    ITenant tenant)
    : ControllerBase
{
    private readonly ILogger<AccountController> _logger = logger;

    [HttpGet("TimeZones")]
    public IActionResult GetTimeZones()
    {
        var timeZones = accountSettingsService.GetTimeZones();

        return Ok(timeZones);
    }

    [HttpGet("User")]
    public async Task<IActionResult> GetSettings()
    {
        var userId = tenant.UserId;
        var profile = await accountSettingsService.GetUserProfile(userId);

        return Ok(profile);
    }

    [HttpPost("User")]
    public async Task<IActionResult> CreateUserProfile(UserCreateViewModel viewModel)
    {
        var createdProfile = await accountSettingsService.CreateUserProfile(viewModel);

        return Ok(createdProfile);
    }

    [HttpPut("User")]
    public async Task<IActionResult> UpdateUserProfile(UserUpdateViewModel viewModel)
    {
        viewModel.UserId = tenant.UserId;
        var updatedProfile = await accountSettingsService.UpdateUserProfile(viewModel);

        return Ok(updatedProfile);
    }
}
