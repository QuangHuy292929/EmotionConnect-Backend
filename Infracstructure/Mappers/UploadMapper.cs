using Application.DTOs.Message;
using Microsoft.AspNetCore.Http;

namespace Infracstructure.Mappers;

public static class UploadMapper
{
    public static UploadFileRequestDto ToUploadRequest(IFormFile file)
    {
        return new UploadFileRequestDto
        {
            Content = file.OpenReadStream(),
            FileName = file.FileName,
            FileSize = file.Length,
            ContentType = file.ContentType
        };
    }

    public static FileUploadResponseDto ToDto(
        string url,
        string fileName,
        long fileSize,
        string fileType) => new()
        {
            Url = url,
            FileName = fileName,
            FileSize = fileSize,
            FileType = fileType
        };
}