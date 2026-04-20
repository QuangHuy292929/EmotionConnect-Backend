using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    //xảy ra xung đột trạng thái giữa request và dữ liệu hiện tại trên server.
    //Ví dụ như cố gắng tạo tài khoản bằng email đã đăng ký, cập nhật tài nguyên đang ở trạng thái không cho phép
    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}
