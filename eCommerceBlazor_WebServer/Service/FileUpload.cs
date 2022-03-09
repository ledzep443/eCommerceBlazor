using Microsoft.AspNetCore.Components.Forms;
using System.IO;
using eCommerceBlazor_WebServer.Service.IService;

namespace eCommerceBlazor_WebServer.Service
{
    public class FileUpload : IFileUpload
    {
        private readonly IWebHostEnvironment _webHostEnviornment;

        public FileUpload(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnviornment = webHostEnvironment;
        }

        public bool DeleteFile(string filePath)
        {
            if (File.Exists(_webHostEnviornment.WebRootPath+filePath))
            {
                File.Delete(_webHostEnviornment.WebRootPath+filePath);
                return true;
            }
            return false;
        }

        public async Task<string> UploadFile(IBrowserFile file)
        {
            FileInfo fileInfo = new(file.Name);
            var fileName = Guid.NewGuid().ToString()+fileInfo.Extension;
            var folderDirectory = $"{_webHostEnviornment.WebRootPath}\\images\\product";
            if (!Directory.Exists(folderDirectory))
            {
                Directory.CreateDirectory(folderDirectory);
            }
            var filePath = Path.Combine(folderDirectory, fileName);

            await using FileStream fs = new FileStream(filePath, FileMode.Create);
            await file.OpenReadStream().CopyToAsync(fs);

            var fullPath = $"/images/product/{fileName}";
            return fullPath;
        }
    }
}
