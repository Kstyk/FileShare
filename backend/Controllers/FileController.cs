using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/file")]
    [ApiController]
    public class FileController : ControllerBase
    {
        private IFileService _fileService;
        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }


        

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile([FromForm] IFormFile File)
        {
            try
            {
                var filePath = await _fileService.UploadFileAsync(File);
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
    }
}
