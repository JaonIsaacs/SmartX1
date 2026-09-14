namespace SmartX.Models;

/// <summary>
/// Represents a file attachment (config, photo, or log) associated with a sensor.
/// </summary>
public class FileAttachment
{
    public int Id { get; set; }

    /// <summary>
    /// Device ID this file is attached to.
    /// </summary>
    public string DeviceId { get; set; } = string.Empty;

    /// <summary>
    /// Original file name.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    /// File extension (config, photo, log).
    /// </summary>
    public string FileType { get; set; } = string.Empty; 

    /// <summary>
    /// Stored file path or URL.
    /// </summary>
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// File size in bytes.
    /// </summary>
    public long FileSizeBytes { get; set; }

    /// <summary>
    /// When the file was uploaded.
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Description of what the file contains.
    /// </summary>
    public string Description { get; set; } = string.Empty;
}