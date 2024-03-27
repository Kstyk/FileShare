using backend.Services;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;

namespace backend.Controllers
{
    [Route("api/files")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile file)
        {
            try
            {
                var filePath = await _fileService.UploadFileAsync(file);
                return Ok(filePath);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

        }

        [HttpPost("upload-multiple")]
        public async Task<IActionResult> UploadFiles([FromForm] IFormFile[] files)
        {
            try
            {
                var filePaths = await _fileService.UploadFilesAsync(files);
                return Ok(filePaths);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllFiles()
        {
            try
            {
                var files = await _fileService.GetAllFilesAsync();
                return Ok(files);
            } catch(UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
            
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(int fileId)
        {
            try
            {
                await _fileService.DeleteFileAsync(fileId);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteFiles([FromQuery] int[] fileIds)
        {
            try
            {
                await _fileService.DeleteFilesAsync(fileIds);
                return Ok();
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> GetFile(int fileId)
        {
            try
            {
                var file = await _fileService.GetFileByIdAsync(fileId);
                return Ok(file);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("download/{fileId}")]
        public async Task<IActionResult> DownloadFile(int fileId)
        {
            try
            {
                var file = await _fileService.GetFileByIdAsync(fileId);
                var memory = new MemoryStream();
                using (var stream = new FileStream(file.Path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                return File(memory, "application/octet-stream", file.Name);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("download-multiple")]
        public async Task<IActionResult> DownloadFiles([FromQuery] int[] fileIds)
        {
            try
            {
                var memory = new MemoryStream();
                using (var zipArchive = new ZipArchive(memory, ZipArchiveMode.Create, true))
                {
                    foreach (var fileId in fileIds)
                    {
                        var file = await _fileService.GetFileByIdAsync(fileId);
                        var entry = zipArchive.CreateEntry(file.Name);
                        using (var entryStream = entry.Open())
                        {
                            using (var stream = new FileStream(file.Path, FileMode.Open))
                            {
                                await stream.CopyToAsync(entryStream);
                            }
                        }
                    }
                }
                memory.Position = 0;
                return File(memory, "application/octet-stream", "files.zip");
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("update-public/{fileId}")]
        public async Task<IActionResult> UpdateFilePublic(int fileId, [FromQuery] bool isPublic)
        {
            try
            {
                var file = await _fileService.ChangeFilePublicAsync(fileId, isPublic);
                return Ok(file);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
