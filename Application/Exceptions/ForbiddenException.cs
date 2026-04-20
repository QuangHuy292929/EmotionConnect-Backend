using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions;

//Người dùng đã đăng nhập nhưng không có quyền thực hiện hành động đó
//Ví dụ như cố gắng xóa thông tin không phải của bản thân
public class ForbiddenException : AppException
{
    public ForbiddenException(string message) : base(message)
    {
    }
}
