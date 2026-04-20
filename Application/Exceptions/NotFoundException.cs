using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions;

//Sử dụng khi tài nguyên không tồn tại theo Id hoặc điều kiện tìm kiếm
//Ví dụ như không tìm ra room bằng roomId đã cho
public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
