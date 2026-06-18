namespace GymManagement.BLL.Services.Attachment
{
    public interface IAttachmentService
    {
        Task<string?> UploadFileAsync(Stream fileStream,string fileName,string folderName, CancellationToken ct = default);
    
        bool Delete(string fileName,string folderName);
        
        (Stream stream,string contentType)? GetFile(string fileName,string folderName);
    }
}
