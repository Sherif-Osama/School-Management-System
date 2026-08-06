using Microsoft.AspNetCore.Mvc;
using School.BLL.Interfaces;
using School.DTO.AuthDTOs;

namespace School.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<LoginResponseDTO>> Login(LoginRequestDTO request)
        {
            return Ok(await _authService.LoginAsync(request));
        }
    }
}
