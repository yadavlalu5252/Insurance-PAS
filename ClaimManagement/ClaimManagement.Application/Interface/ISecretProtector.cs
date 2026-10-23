using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface ISecretProtector
    {
        string Protect(string value);
        string Unprotect(string value);
    }
}
