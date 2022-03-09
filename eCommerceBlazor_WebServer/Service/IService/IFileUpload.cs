using Microsoft.AspNetCore.Components.Forms;

namespace eCommerceBlazor_WebServer.Service.IService
{
    public interface IFileUpload
    {
        Task<string> UploadFile(IBrowserFile file);

        bool DeleteFile(string filePath);
    }
}
