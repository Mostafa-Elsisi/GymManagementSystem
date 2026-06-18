using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GymManagement.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long _maxFileSize = 5 * 1024 * 1024; // 5MB limit
        private readonly ILogger<AttachmentService> _logger;
        private readonly IWebHostEnvironment _env;
        private readonly string[] _allowedExtension = { "png", "jpeg", "jpg", "PNG", "JPEG", "JPG" };

        public AttachmentService(ILogger<AttachmentService> logger, IWebHostEnvironment env)
        {
            _logger = logger;
            _env = env;
        }

        public bool Delete(string fileName, string folderName)
        {
            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            try
            {
                if (!File.Exists(fullPath))
                    return false;
                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to delete file {fileName} in folder {folderName}");
                return false;
            }

        }

        public (Stream stream, string contentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
                return null;

            var fullPath = Path.Combine(_env.ContentRootPath, folderName, fileName);
            if (!File.Exists(fullPath))
                return null;

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read);

            var extension = Path.GetFullPath(fullPath).ToLower();
            var contentType = extension switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                _ => "application/octet-stream"
            };
            return (stream, contentType);
        }

        public async Task<string?> UploadFileAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct = default)
        {

            if (fileStream == null || fileStream.Length == 0 || !fileStream.CanRead)
                return null;

            // 1 : Check the size — reject anything over 5 MB.
            if (fileStream.Length > _maxFileSize)
            {
                _logger.LogError($"File Rejected : File To Large {fileStream.Length} Bytes");
                return null;
            }

            // 2 : Check the extension — only.jpg.jpeg.png allowed.

            var extension = Path.GetExtension(fileName).TrimStart('.');

            if (string.IsNullOrWhiteSpace(extension) || !_allowedExtension.Contains(extension))
            {
                _logger.LogError($"File Rejected : File Extension {extension} Not Allowed");
                return null;
            }

            // 3 : Locate the folder & create it if missing.
            var uploadFolder = Path.Combine(_env.ContentRootPath, folderName);
            Directory.CreateDirectory(uploadFolder);

            // 4 : Make the name unique using a GUID
            var storedFileName = $"{Guid.NewGuid()} {fileName}";

            // 5 : Build the full file path.
            var filePath = Path.Combine(uploadFolder, storedFileName);

            try
            {
                // 6 : Open a file stream(an unmanaged resource).
                using var fs = new FileStream(filePath, FileMode.Create, FileAccess.Write);

                // 7 : Copy the file into that stream.
                await fileStream.CopyToAsync(fs, ct);

                // 8 : Return the file name to store in the database.
                return storedFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed To Upload File {fileName}");
                return null;
            }
        }
    }
}
