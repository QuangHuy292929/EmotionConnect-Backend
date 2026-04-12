using Application.DTOs.Reflection;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class ReflectionController : ControllerBase
{
    private readonly IReflectionService _reflectionService;
    public ReflectionController(IReflectionService reflectionService)
    {
        _reflectionService = reflectionService;
    }
    [HttpPost]
    public async Task<ActionResult<ReflectionDto>> Create(CreateReflectionRequest request, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        var reflection = await _reflectionService.CreateAsync(request, userId, ct);
        return Ok(reflection);
    }

    [HttpGet("room/{roomId:guid}")]
    public async Task<ActionResult<List<ReflectionDto>>> GetByRoomId(Guid roomId, CancellationToken ct)
    {
        var reflections = await _reflectionService.GetByRoomAsync(roomId, ct);
        return Ok(reflections);
    }

    [HttpGet("my")]
    public async Task<ActionResult<List<ReflectionDto>>> GetMine(CancellationToken cancellationToken)
    {
        var userId = User.GetCurrentUserId();
        var result = await _reflectionService.GetMyReflectionsAsync(userId, cancellationToken);
        return Ok(result);
    }
}
