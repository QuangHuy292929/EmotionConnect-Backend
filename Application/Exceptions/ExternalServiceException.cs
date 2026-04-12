using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions
{
    public class ExternalServiceException : AppException
    {
        public ExternalServiceException(string message) : base(message)
        {
        }
    }
}
