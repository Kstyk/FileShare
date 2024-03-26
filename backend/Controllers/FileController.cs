using backend.Services;
using Microsoft.AspNetCore.Mvc;

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
    }
}
