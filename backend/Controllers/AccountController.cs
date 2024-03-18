using backend.Models;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        public AccountController()
        {
            
        }

        public ActionResult RegisterUser([FromBody] RegisterUserDto dto)
        {
            return Ok();
        }
    }
}
