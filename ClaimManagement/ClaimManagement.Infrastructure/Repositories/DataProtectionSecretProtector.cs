using ClaimManagement.Application.Interface;
using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class DataProtectionSecretProtector : ISecretProtector
    {
        IDataProtector protector;

        public DataProtectionSecretProtector(IDataProtectionProvider provider)
        {
            protector = provider.CreateProtector("ClaimManagement.Settlement.v1");
        }

        public string Protect(string value)
        {
            return protector.Protect(value);
        }

        public string Unprotect(string value)
        {
            return protector.Unprotect(value);
        }
    }
}
