using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital.BBL.Services.AttachementService
{
    public interface IAttachementService
    {
        public string? Upload(IFormFile file,string FolderName);
        public bool Delete(string FilePath);
    }
}
