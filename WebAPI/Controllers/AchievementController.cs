using Application.DTOs.Achievement;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class AchievementController : ControllerBase
{
    private readonly IAchievementService _achievementService;
    public AchievementController(IAchievementService achievementService)
    {
        _achievementService = achievementService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AchievementDto>>> GetAllAchievements(CancellationToken ct)
    {
        var achievements = await _achievementService.GetActiveAchievementsAsync(ct);
        return Ok(achievements);
    }

    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<UserAchievementDto>>> GetMyAchievements(CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var achievements = await _achievementService.GetMyAchievementsAsync(userId, ct);
        return Ok(achievements);
    }

    [HttpGet("me/{achievementId:guid}")]
    public async Task<ActionResult<UserAchievementDto>> GetMyAchievementById(Guid achievementId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var achievement = await _achievementService.GetUserAchievementAsync(userId, achievementId, ct);
        if (achievement == null)
        {
            return NotFound();
        }
        return Ok(achievement);
    }

    [HttpGet("me/code/{code}")]
    public async Task<ActionResult<UserAchievementDto>> GetMyAchievementByCode(string code, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var achievement = await _achievementService.GetUserAchievementByCodeAsync(userId, code, ct);
        if (achievement == null)
        {
            return NotFound();
        }
        return Ok(achievement);
    }

    [HttpPost("me/{achievementId:guid}/initialize")]
    public async Task<ActionResult<UserAchievementDto>> InitializeProgress(Guid achievementId, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var achievement = await _achievementService.InitializeProgressAsync(userId, achievementId, ct);
        return Ok(achievement);
    }

    [HttpPost("me/code/{code}/increment")]
    public async Task<ActionResult<UserAchievementDto>> IncrementProgress(string code, int amount, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var achievement = await _achievementService.IncrementProgressAsync(userId, code, amount, ct);
        return Ok(achievement);
    }

    [HttpPost("me/code/{code}/set-progress")]
    public async Task<ActionResult<UserAchievementDto>> SetProgress(string code, int progressValue, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var achievement = await _achievementService.SetProgressAsync(userId, code, progressValue, ct);
        return Ok(achievement);
    }

    [HttpPost("me/code/{code}/unlock")]
    public async Task<ActionResult<UserAchievementDto>> Unlock(string code, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var achievement = await _achievementService.UnlockAsync(userId, code, ct);
        return Ok(achievement);
    }
}