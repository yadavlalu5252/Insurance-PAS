using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IFileStorageService
    {
        Task<string> SaveFileAsync(Stream content, string extension);
    }
}
