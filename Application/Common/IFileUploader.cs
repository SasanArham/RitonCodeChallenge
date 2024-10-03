using Microsoft.AspNetCore.Http;

namespace Application.Common
{
    public interface IFileUploader
    {
        Task<string> UploadAsync(IFormFile formFile, string destinationPath);
    }
}
