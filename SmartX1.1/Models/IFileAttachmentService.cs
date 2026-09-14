using SmartX.Models;

namespace SmartX.Services;
/// <summary>
/// not removing this but it is not used, may implement later 
/// </summary>
public interface IFileAttachmentService
{
    /// <summary>
    /// Store a file attachment for a device.
    /// </summary>
    Task<FileAttachment> StoreFileAsync(string deviceId, string fileName, string fileType, Stream fileStream, string description = "");

    /// <summary>
    /// Store multiple file attachments for a device in one call.
    /// </summary>
    Task<IReadOnlyList<FileAttachment>> StoreFilesAsync(string deviceId, string fileType, IEnumerable<(string FileName, Stream Content)> files, string description = "");

    /// <summary>
    /// Get all attachments for a device.
    /// </summary>
    Task<IEnumerable<FileAttachment>> GetDeviceAttachmentsAsync(string deviceId);

    /// <summary>
    /// Get attachment by ID.
    /// </summary>
    Task<FileAttachment?> GetAttachmentAsync(int attachmentId);

    /// <summary>
    /// Delete an attachment.
    /// </summary>
    Task DeleteAttachmentAsync(int attachmentId);

    /// <summary>
    /// Get file content for download.
    /// </summary>
    Task<Stream?> GetFileStreamAsync(int attachmentId);
}