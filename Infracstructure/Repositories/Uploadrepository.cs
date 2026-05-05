
using Application.Interfaces.IRepositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Infracstructure.Repositories;

public class UploadRepository : IUploadRepository
{
    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UploadRepository(IWebHostEnvironment env, IHttpContextAccessor httpContextAccessor)
    {
        _env = env;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<string> SaveFileAsync(
        Stream stream,
        string savedFileName,
        string subFolder,
        CancellationToken ct = default)
    {
        var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
        var uploadDir = Path.Combine(webRoot, "uploads", subFolder);
        Directory.CreateDirectory(uploadDir);

        var savePath = Path.Combine(uploadDir, savedFileName);
        await using var output = File.Create(savePath);
        await stream.CopyToAsync(output, ct);

        var request = _httpContextAccessor.HttpContext!.Request;
        return $"{request.Scheme}://{request.Host}/uploads/{subFolder}/{savedFileName}";
    }

    public Task DeleteFileAsync(string fileUrl, CancellationToken ct = default)
    {
        try
        {
            var uri = new Uri(fileUrl);
            var webRoot = _env.WebRootPath ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(webRoot, uri.AbsolutePath.TrimStart('/').Replace('/', Path.DirectorySeparatorChar));

            if (File.Exists(filePath))
                File.Delete(filePath);
        }
        catch
        {
            // Silent — file có thể đã bị xoá trước đó
        }

        return Task.CompletedTask;
    }
}