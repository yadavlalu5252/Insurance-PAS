using ClaimManagement.Application.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Infrastructure.Repositories
{
    public class LocalFileStorageService : IFileStorageService
    {
        string rootPath;

        public LocalFileStorageService(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public async Task<string> SaveFileAsync(Stream content, string extension)
        {
            Directory.CreateDirectory(rootPath);

            // random name: the user's file name is never used on disk
            string storedName = Guid.NewGuid().ToString("N") + extension;
            string fullPath = Path.Combine(rootPath, storedName);

            using var fileStream = new FileStream(fullPath, FileMode.Create);
            await content.CopyToAsync(fileStream);

            return storedName;
        }
    }
}
