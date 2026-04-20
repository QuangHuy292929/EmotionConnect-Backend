using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    //Sử dụng khi gọi service bên ngoài nhưng trả về lỗi hoặc thất bại
    //Ví dụ như gọi đến Ai service nhưng không có hồi đáp
    public class ExternalServiceException : AppException
    {
        public ExternalServiceException(string message) : base(message)
        {
        }
    }
}
