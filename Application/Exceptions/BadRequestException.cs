using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message)
        {
        }
    }
}
