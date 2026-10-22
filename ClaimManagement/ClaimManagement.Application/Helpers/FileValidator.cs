using ClaimManagement.Application.Exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace ClaimManagement.Application.Helpers
{
    public static class FileValidator
    {
        // Recommended limits (not in the BRD)
        static readonly string[] AllowedExtensions = { ".pdf", ".jpg", ".jpeg", ".png" };
        const long MaxFileSize = 5 * 1024 * 1024;   // 5 MB

        // returns the lower-case extension, or throws BadRequestException
        public static string Check(string? fileName, long fileLength, Stream? content)
        {
            if (content == null || fileLength == 0 || string.IsNullOrWhiteSpace(fileName))
                throw new BadRequestException("File is required");

            if (fileLength > MaxFileSize)
                throw new BadRequestException("File size cannot be more than 5 MB");

            string extension = Path.GetExtension(fileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
                throw new BadRequestException("Only pdf, jpg, jpeg and png files are allowed");

            return extension;
        }
    }
}
