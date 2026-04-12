using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
