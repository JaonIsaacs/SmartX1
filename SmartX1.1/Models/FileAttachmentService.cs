using System.Security.Cryptography;
using SmartX.Models;

namespace SmartX.Services;

/// <summary>
/// Manages file attachments (configs, photos, logs) for sensor devices.
/// Files are streamed directly to disk (no full in-memory buffering) and
/// encrypted at rest using AES-256. Uses in-memory metadata storage for
/// development; can be swapped with cloud storage (Azure Blob, S3).
/// </summary>
public class FileAttachmentService : IFileAttachmentService
{
    private readonly List<FileAttachment> _attachments = new();
    private int _nextId = 1;
    private readonly string _uploadDir;
    private readonly byte[] _encryptionKey;
    private readonly object _lock = new();

    private static readonly string[] ValidFileTypes = { "config", "photo", "log" };

    public FileAttachmentService(IConfiguration configuration)
    {
        _uploadDir = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        if (!Directory.Exists(_uploadDir))
        {
            Directory.CreateDirectory(_uploadDir);
        }

        // Derive a stable 256-bit key from configuration (falls back to a dev default).
        var keySeed = configuration["FileEncryption:Key"] ?? "SmartX1.1-Dev-Key-Change-In-Production";
        _encryptionKey = SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(keySeed));
    }

    public async Task<FileAttachment> StoreFileAsync(string deviceId, string fileName, string fileType, Stream fileStream, string description = "")
    {
        if (string.IsNullOrWhiteSpace(deviceId))
            throw new ArgumentException("DeviceId is required");

        if (fileStream == null || fileStream.Length == 0)
            throw new ArgumentException("File stream cannot be empty");

        if (!ValidFileTypes.Contains(fileType.ToLowerInvariant()))
            throw new ArgumentException($"Invalid file type. Must be one of: {string.Join(", ", ValidFileTypes)}");

        var uniqueFileName = $"{deviceId}_{Guid.NewGuid()}_{fileName}.enc";
        var filePath = Path.Combine(_uploadDir, uniqueFileName);

        await EncryptToDiskAsync(fileStream, filePath);

        var attachment = new FileAttachment
        {
            FileName = fileName,
            FileType = fileType.ToLowerInvariant(),
            DeviceId = deviceId,
            FilePath = filePath,
            FileSizeBytes = fileStream.Length,
            Description = description,
            UploadedAt = DateTime.UtcNow
        };

        lock (_lock)
        {
            attachment.Id = _nextId++;
            _attachments.Add(attachment);
        }

        return attachment;
    }

    /// <summary>
    /// Uploads multiple files for a device in a single call, streaming each
    /// one independently so large batches don't degrade backend performance.
    /// </summary>
    public async Task<IReadOnlyList<FileAttachment>> StoreFilesAsync(
        string deviceId, string fileType, IEnumerable<(string FileName, Stream Content)> files, string description = "")
    {
        var results = new List<FileAttachment>();

        foreach (var (fileName, content) in files)
        {
            var attachment = await StoreFileAsync(deviceId, fileName, fileType, content, description);
            results.Add(attachment);
        }

        return results;
    }

    public Task<IEnumerable<FileAttachment>> GetDeviceAttachmentsAsync(string deviceId)
    {
        lock (_lock)
        {
            var attachments = _attachments
                .Where(a => a.DeviceId == deviceId)
                .OrderByDescending(a => a.UploadedAt)
                .ToList();

            return Task.FromResult(attachments.AsEnumerable());
        }
    }

    public Task<FileAttachment?> GetAttachmentAsync(int attachmentId)
    {
        lock (_lock)
        {
            return Task.FromResult(_attachments.FirstOrDefault(a => a.Id == attachmentId));
        }
    }

    public Task DeleteAttachmentAsync(int attachmentId)
    {
        lock (_lock)
        {
            var attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
            if (attachment != null)
            {
                if (File.Exists(attachment.FilePath))
                {
                    File.Delete(attachment.FilePath);
                }
                _attachments.Remove(attachment);
            }
        }

        return Task.CompletedTask;
    }

    public async Task<Stream?> GetFileStreamAsync(int attachmentId)
    {
        FileAttachment? attachment;
        lock (_lock)
        {
            attachment = _attachments.FirstOrDefault(a => a.Id == attachmentId);
        }

        if (attachment == null || !File.Exists(attachment.FilePath))
            return null;

        return await DecryptToMemoryStreamAsync(attachment.FilePath);
    }

    /// <summary>
    /// Streams the source content through an AES encryptor directly to disk avoiding loading the whole file into memory at once.
    /// </summary>
    private async Task EncryptToDiskAsync(Stream source, string destinationPath)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;
        aes.GenerateIV();

        await using var fileOut = new FileStream(destinationPath, FileMode.Create, FileAccess.Write);

        // Prefix the file with the IV so it can be decrypted later.
        await fileOut.WriteAsync(aes.IV);

        await using var cryptoStream = new CryptoStream(fileOut, aes.CreateEncryptor(), CryptoStreamMode.Write, leaveOpen: true);
        await source.CopyToAsync(cryptoStream);
    }

    /// <summary>
    /// Decrypts a file from disk into an in-memory stream for download.
    /// </summary>
    private async Task<Stream> DecryptToMemoryStreamAsync(string sourcePath)
    {
        using var aes = Aes.Create();
        aes.Key = _encryptionKey;

        await using var fileIn = new FileStream(sourcePath, FileMode.Open, FileAccess.Read);

        var iv = new byte[aes.BlockSize / 8];
        await fileIn.ReadExactlyAsync(iv);
        aes.IV = iv;

        var output = new MemoryStream();
        await using (var cryptoStream = new CryptoStream(fileIn, aes.CreateDecryptor(), CryptoStreamMode.Read))
        {
            await cryptoStream.CopyToAsync(output);
        }

        output.Position = 0;
        return output;
    }
}