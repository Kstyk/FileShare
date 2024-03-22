using backend.Models;
using backend.Models.Responses;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private IAccountService _accountService;
        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            _accountService.RegisterUser(dto);
            return Ok();
        }

        [HttpPost("login")]
        public ActionResult LoginUser([FromBody] LoginUserDto dto)
        {
            try
            {
                LoginResponseDto tokens = _accountService.LoginUser(dto);
                return Ok(tokens);

            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(ex.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                return BadRequest(ex.Message);
            } 
        }

        [HttpPost("refresh-token")]
        public ActionResult<RefreshTokenResponseDto> RefreshToken([FromBody] RefreshTokenDto refreshTokenDto)
        {
            try
            {
                var newAccessToken = _accountService.RefreshAccessToken(refreshTokenDto.RefreshToken);
                return Ok(newAccessToken);
            } catch(Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.InnerException?.Message);
                return BadRequest(ex.Message);
            }
        }
    }
}
