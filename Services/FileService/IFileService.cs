namespace DoAn4.Services.FileService
{
    public interface IFileService
    {
        Task<string> SaveFile(IFormFile File);   
        Task<bool> DeleteFiles(List<string> filePath);
    }
}
