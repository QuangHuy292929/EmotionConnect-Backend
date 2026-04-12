using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    public class UnauthorizeException : AppException
    {
        public UnauthorizeException(string message) : base(message)
        {
        }
    }
}
