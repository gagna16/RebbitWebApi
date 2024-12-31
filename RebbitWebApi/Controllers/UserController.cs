using Microsoft.AspNetCore.Mvc;
using RebbitWebApi.Dto;
using RebbitWebApi.Service;

namespace RebbitWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        //// რეგისტრაცია
        //[HttpPost("register")]
        //public async Task<IActionResult> Register([FromBody] RegistrationDto registrationDto)
        //{
        //    try
        //    {
        //        var user = await _userService.RegisterUserAsync(registrationDto);
        //        var token = await _userService.GenerateJwtToken(user); // JWT ტოკენი რეგისტრაციის შემდეგ
        //        return Ok(new { token });
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(new { message = ex.Message });
        //    }
        //}

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationDto registrationDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid input");
            }

            try
            {
                // შენი ბიზნეს ლოგიკა აქ
                var user = await _userService.RegisterUserAsync(registrationDto);
                var token = await _userService.GenerateJwtToken(user); // JWT ტოკენი რეგისტრაციის შემდეგ
                return Ok(new { token });
                //return Ok("User registered successfully");
            }
            catch (Exception ex)
            {
                // შეცდომის დეტალების დაბრუნება (Debugging ეტაპზე)
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        // ლოგინი
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            try
            {
                var token = await _userService.LoginUserAsync(loginDto);
                return Ok(new { token });
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpGet("getData")]
        public async Task<IActionResult> GetData()
        {
            try
            {
                var users = _userService.GetUser();
                return Ok(users);
            }
            catch (Exception ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
        }
        [HttpPost("test")]
        public IActionResult Test(string name)
        {
            return Ok(name);
        }

        // JWT-თან დაკავშირებული "logout" (JWT ტოკენის წაშლა კლიანტ-გვერდზე)
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // კლიენტს უნდა უთხრას, რომ წაშალოს ტოკენი (თუ გამოიყენება LocalStorage ან SessionStorage)
            return Ok(new { message = "Logged out successfully. Please remove your JWT token from localStorage." });
        }
    }
}
