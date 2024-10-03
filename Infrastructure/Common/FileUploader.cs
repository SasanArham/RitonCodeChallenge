using Application.Common;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Common
{
    public class FileUploader : IFileUploader
    {
        public async Task<string> UploadAsync(IFormFile formFile, string destinationPath)
        {
            var uniqueName = $"{Guid.NewGuid()}_{formFile.FileName}";
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), destinationPath, uniqueName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await formFile.CopyToAsync(stream);
            }
            return uniqueName;
        }
    }
}
