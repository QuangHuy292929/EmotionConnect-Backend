using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTOs.Message
{
    public class FileUploadResponseDto
    {
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public long FileSize { get; set; }
        public string FileType { get; set; } = string.Empty; // "image" | "file"
    }
}
