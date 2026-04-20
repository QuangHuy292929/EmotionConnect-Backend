using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    //Sử dụng khi request từ client không hợp lệ, dữ liệu sai format, validation thất bại, hoặc thiếu thông tin bắt buộc.
    //Ví dụ: Client gửi email không đúng định dạng.
    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
