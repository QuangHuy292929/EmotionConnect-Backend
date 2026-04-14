using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message)
        {
        }
    }
}
