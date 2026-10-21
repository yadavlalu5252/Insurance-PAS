using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Interface
{
    public interface IFileStorageService
    {
        // saves the file and returns the stored (random) file name
        Task<string> SaveFileAsync(Stream content, string extension);
    }
}
