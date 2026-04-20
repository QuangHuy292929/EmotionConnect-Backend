using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    //Sử dụng khi người dùng chưa xác thực
    //Ví dụ như người dùng chưa login, token sai, token không đúng định dạng
    public class UnauthorizeException : AppException
    {
        public UnauthorizeException(string message) : base(message)
        {
        }
    }
}
