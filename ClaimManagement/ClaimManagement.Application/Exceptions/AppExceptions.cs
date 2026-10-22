using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException(string message) : base(message) 
        { 
        }
    }

    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) 
        { 
        }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) 
        { 
        }
    }
}
