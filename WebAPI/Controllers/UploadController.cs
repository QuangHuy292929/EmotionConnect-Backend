using Application.DTOs.Message;
using Application.Interfaces.IServices;
using Infracstructure.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Infracstructure.Mappers;

namespace WebAPI.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UploadController : ControllerBase
{
    private readonly IUploadService _uploadService;

    public UploadController(IUploadService uploadService)
    {
        _uploadService = uploadService;
    }

    [HttpPost("image")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FileUploadResponseDto>> UploadImage(IFormFile file, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        await using var stream = file.OpenReadStream();
        var request = UploadMapper.ToUploadRequest(file);

        var result = await _uploadService.UploadImageAsync(request, userId, ct);

        return Ok(result);
    }

    [HttpPost("file")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<FileUploadResponseDto>> UploadFile(IFormFile file, CancellationToken ct)
    {
        var userId = User.GetCurrentUserId();
        await using var stream = file.OpenReadStream();
        var request = UploadMapper.ToUploadRequest(file);

        var result = await _uploadService.UploadFileAsync(request, userId, ct);

        return Ok(result);
    }

   
}
