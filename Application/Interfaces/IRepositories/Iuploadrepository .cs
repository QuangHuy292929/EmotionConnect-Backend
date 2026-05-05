using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces.IRepositories
{
    public interface IUploadRepository
    {
        Task<string> SaveFileAsync(Stream stream, string savedFileName, string subFolder, CancellationToken ct = default);
        Task DeleteFileAsync(string fileUrl, CancellationToken ct = default);
    }

}
