using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace Hospital.BBL.Services.AttachementService
{
    public class AttachementService : IAttachementService
    {
        List<string> AllowedExtensions = new List<string> { ".jpg", ".jpeg", ".png" };
        const int MaxSize = 2_097_152;
        public string? Upload(IFormFile file, string FolderName)
        {
            if (file is null || file.Length == 0)
                return null;

            var extension = Path.GetExtension(file.FileName)?.ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension) || !AllowedExtensions.Contains(extension))
                return null;

            if (file.Length > MaxSize)
                return null;

            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Files", FolderName);
            Directory.CreateDirectory(folderPath);

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(file.FileName)}";
            var fullPath = Path.Combine(folderPath, fileName);

            using FileStream fs = new FileStream(fullPath, FileMode.Create);
            file.CopyTo(fs);

            return fileName;
        }

        public bool Delete(string FilePath)
        {
            if(!File.Exists(FilePath)) return false;
            else
            {
                File.Delete(FilePath);
            }
            return true;
        }
    }
}
