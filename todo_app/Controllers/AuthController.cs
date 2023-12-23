using Microsoft.AspNetCore.Mvc;
using todo_app.core.Models.Auth;
using todo_app.core.Models.ResponseModels.Auth;
using todo_app.core.Services;

namespace todo_app.api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterModel model)
        {
            var result = await _authService.RegisterAsync(model);

            if(!result.IsAuthenticated)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginModel model)
        {

            var result = await _authService.LoginAsync(model);
            if(!result.IsAuthenticated)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}
