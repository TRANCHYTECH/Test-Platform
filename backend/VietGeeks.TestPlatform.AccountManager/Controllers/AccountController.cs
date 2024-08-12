﻿using Microsoft.AspNetCore.Authorization;
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
public class AccountController : ControllerBase
{
    private readonly ILogger<AccountController> _logger;
    private readonly IAccountSettingsService _accountSettingsService;
    private readonly ITenant _tenant;

    public AccountController(ILogger<AccountController> logger, IAccountSettingsService accountSettingsService, ITenant tenant)
    {
        _logger = logger;
        _accountSettingsService = accountSettingsService;
        _tenant = tenant;
    }

    [HttpGet("TimeZones")]
    public IActionResult GetTimeZones()
    {
        var timeZones = _accountSettingsService.GetTimeZones();

        return Ok(timeZones);
    }

    [HttpGet("User")]
    public async Task<IActionResult> GetSettings()
    {
        var userId = _tenant.UserId;
        var profile = await _accountSettingsService.GetUserProfile(userId);

        return Ok(profile);
    }

    [HttpPost("User")]
    public async Task<IActionResult> CreateUserProfile(UserCreateViewModel viewModel)
    {
        var createdProfile = await _accountSettingsService.CreateUserProfile(viewModel);

        return Ok(createdProfile);
    }

    [HttpPut("User")]
    public async Task<IActionResult> UpdateUserProfile(UserUpdateViewModel viewModel)
    {
        viewModel.UserId = _tenant.UserId;
        var updatedProfile = await _accountSettingsService.UpdateUserProfile(viewModel);

        return Ok(updatedProfile);
    }
}
