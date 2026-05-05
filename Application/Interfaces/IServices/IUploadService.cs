using Application.DTOs.Message;

namespace Application.Interfaces.IServices
{
    public interface IUploadService
    {
        Task<FileUploadResponseDto> UploadImageAsync(UploadFileRequestDto file, Guid userId, CancellationToken ct = default);
        Task<FileUploadResponseDto> UploadFileAsync(UploadFileRequestDto file, Guid userId, CancellationToken ct = default);
    }
}
