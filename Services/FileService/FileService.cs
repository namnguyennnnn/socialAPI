using CloudinaryDotNet.Actions;
using CloudinaryDotNet;
using DoAn4.Services.FileService;

public class FileService : IFileService
{
    private readonly Cloudinary _cloudinary;

    public FileService(IConfiguration configuration)
    {
        string cloudName = configuration["CloudinarySettings:CloudName"];
        string apiKey = configuration["CloudinarySettings:ApiKey"];
        string apiSecret = configuration["CloudinarySettings:ApiSecret"];

        Account account = new Account(cloudName, apiKey, apiSecret);
        _cloudinary = new Cloudinary(account);
    }

    public async Task<string> SaveFile(IFormFile file)
    {
        string uniqueFileName = $"{Guid.NewGuid()}_{file.FileName}";

        using (var memoryStream = new MemoryStream())
        {
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(uniqueFileName, memoryStream),
                PublicId = "post_images/" + uniqueFileName // Đường dẫn lưu trữ trong Cloudinary
            };

            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            return uploadResult.SecureUri.ToString();
        }
    }

    public async Task<bool> DeleteFiles(List<string> fileUrls)
    {
        foreach (var url in fileUrls)
        {
            string publicId = GetPublicIdFromUrl(url);
            var deletionResult = await _cloudinary.DeleteResourcesByPrefixAsync(publicId);

            if (deletionResult.Deleted.Count == 0)
            {
                Console.WriteLine($"Xóa tài nguyên không thành công: {url}");
                return false; 
            }
            else
            {
                Console.WriteLine($"Đã xóa tài nguyên: {url}");
            }
        }

        return true; 
    }




    private string GetPublicIdFromUrl(string url)
    {
        // Trích xuất public ID từ URL Cloudinary
        Uri uri = new Uri(url);
        string publicId = uri.Segments.Last().Split('.')[0];
        return publicId;
    }
}
