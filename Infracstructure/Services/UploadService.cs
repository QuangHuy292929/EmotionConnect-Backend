using Application.DTOs.Message;
using Application.Exceptions;
using Application.Interfaces.IRepositories;
using Application.Interfaces.IServices;

namespace Infracstructure.Services;

public class UploadService : IUploadService
{
    private readonly IUploadRepository _uploadRepository;

    private static readonly string[] AllowedImages = [".jpg", ".jpeg", ".png", ".gif", ".webp"];
    private static readonly string[] AllowedFiles = [".pdf", ".docx", ".txt", ".zip"];

    private const long MaxImageSize = 10 * 1024 * 1024; // 10MB
    private const long MaxFileSize = 20 * 1024 * 1024; // 20MB

    public UploadService(IUploadRepository uploadRepository)
    {
        _uploadRepository = uploadRepository;
    }

    public async Task<FileUploadResponseDto> UploadImageAsync(
        UploadFileRequestDto file,
        Guid userId,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new BadRequestException("UserId không hợp lệ.");

        Validate(file, AllowedImages, MaxImageSize);

        return await SaveAsync(file, "img", "image", ct);
    }

    public async Task<FileUploadResponseDto> UploadFileAsync(
        UploadFileRequestDto file,
        Guid userId,
        CancellationToken ct = default)
    {
        if (userId == Guid.Empty)
            throw new BadRequestException("UserId không hợp lệ.");

        Validate(file, AllowedFiles, MaxFileSize);

        return await SaveAsync(file, "file", "file", ct);
    }

    private static void Validate(UploadFileRequestDto file, string[] allowedExts, long maxBytes)
    {
        if (file is null || file.Content == Stream.Null || file.FileSize == 0)
            throw new BadRequestException("File không được để trống.");

        if (file.FileSize > maxBytes)
            throw new BadRequestException(
                $"File vượt quá dung lượng tối đa {maxBytes / 1024 / 1024}MB.");

        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExts.Contains(ext))
            throw new BadRequestException(
                $"Định dạng '{ext}' không được hỗ trợ. Cho phép: {string.Join(", ", allowedExts)}");
    }

    private async Task<FileUploadResponseDto> SaveAsync(
        UploadFileRequestDto file,
        string subFolder,
        string fileType,
        CancellationToken ct)
    {
        var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
        var savedFileName = $"{Guid.NewGuid()}{ext}";

        var url = await _uploadRepository.SaveFileAsync(file.Content, savedFileName, subFolder, ct);

        return new FileUploadResponseDto
        {
            Url = url,
            FileName = file.FileName,
            FileSize = file.FileSize,
            FileType = fileType
        };
    }
}
