using Microsoft.AspNetCore.Mvc;
using SmartX.Models;
using SmartX.Services;
using SmartX1._1.Models;

namespace SmartX1._1.Controllers;

[ApiController]
[Route("api/files")]
public class FilesController : ControllerBase
{
    private readonly IFileAttachmentService _fileService;
    private readonly ILogger<FilesController> _logger;

    public FilesController(IFileAttachmentService fileService, ILogger<FilesController> logger)
    {
        _fileService = fileService;
        _logger = logger;
    }

    [HttpPost("upload/{deviceId}")]
    public async Task<IActionResult> Upload(string deviceId, [FromQuery] string fileType, [FromQuery] string? description, IFormFile file)
    {
        try
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { error = "No file was uploaded" });

            await using var stream = file.OpenReadStream();
            var attachment = await _fileService.StoreFileAsync(deviceId, file.FileName, fileType, stream, description ?? string.Empty);

            return Ok(attachment);
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "File upload failed for {DeviceId}", deviceId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error uploading file for {DeviceId}", deviceId);
            return StatusCode(500, new { error = "An unexpected error occurred while uploading the file." });
        }
    }

    /// <summary>
    /// Uploads multiple files for a device in a single multipart request streaming each part independently to avoid buffering the whole batch in memory.
    /// </summary>
    [HttpPost("upload-batch/{deviceId}")]
    [RequestSizeLimit(100_000_000)]
    public async Task<IActionResult> UploadBatch(string deviceId, [FromQuery] string fileType, [FromQuery] string? description, List<IFormFile> files)
    {
        try
        {
            if (files == null || files.Count == 0)
                return BadRequest(new { error = "No files were uploaded" });

            var openStreams = files.Select(f => (f.FileName, Content: f.OpenReadStream())).ToList();

            try
            {
                var attachments = await _fileService.StoreFilesAsync(deviceId, fileType, openStreams, description ?? string.Empty);
                return Ok(attachments);
            }
            finally
            {
                foreach (var (_, content) in openStreams)
                {
                    await content.DisposeAsync();
                }
            }
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Batch file upload failed for {DeviceId}", deviceId);
            return BadRequest(new { error = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error during batch upload for {DeviceId}", deviceId);
            return StatusCode(500, new { error = "An unexpected error occurred while uploading the files." });
        }
    }

    [HttpGet("device/{deviceId}")]
    public async Task<ActionResult<IEnumerable<FileAttachment>>> GetForDevice(string deviceId)
    {
        var files = await _fileService.GetDeviceAttachmentsAsync(deviceId);
        return Ok(files);
    }

    [HttpGet("download/{attachmentId}")]
    public async Task<IActionResult> Download(int attachmentId)
    {
        var attachment = await _fileService.GetAttachmentAsync(attachmentId);
        if (attachment == null)
            return NotFound(new { error = $"Attachment {attachmentId} not found" });

        var stream = await _fileService.GetFileStreamAsync(attachmentId);
        if (stream == null)
            return NotFound(new { error = "File content not found on disk" });

        return File(stream, "application/octet-stream", attachment.FileName);
    }

    [HttpDelete("{attachmentId}")]
    public async Task<IActionResult> Delete(int attachmentId)
    {
        await _fileService.DeleteAttachmentAsync(attachmentId);
        return Ok(new { message = "File deleted successfully" });
    }
}