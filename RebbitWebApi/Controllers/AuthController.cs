using Microsoft.AspNetCore.Mvc;
using RebbitWebApi.Dto;
using RebbitWebApi.Service;

namespace RebbitWebApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;

        public AuthController(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input");
            }

            try
            {
                var user = await _userService.RegisterUserAsync(registrationDto);
                var token = _tokenService.GenerateJwtToken(user);

                return Ok(new
                {
                    Token = token,
                    Message = "User registered successfully"
                });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
