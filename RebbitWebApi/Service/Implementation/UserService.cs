using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using RebbitWebApi.Dto;
using RebbitWebApi.Model;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RebbitWebApi.Service.Implementation
{
    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;
        // კონსტრუქტორი - ინექცია
        public UserService(UserManager<User> userManager, SignInManager<User> signInManager,IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string> GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.NameIdentifier, user.Id),
        };

            // როლები (თუ არსებობს)
            var roles = await _userManager.GetRolesAsync(user);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(1), // ტოკენი გაუქმდება 1 საათში
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<List<User>> GetUser()
        {
            return _userManager.Users.ToList();
        }

        public async Task<string> LoginUserAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null)
            {
                throw new InvalidOperationException("Invalid login attempt.");
            }

            var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

            if (result.Succeeded)
            {
                return await GenerateJwtToken(user); // თუ წარმატებულია, ტოკენი გამოვიტანოთ
            }

            throw new InvalidOperationException("Invalid login attempt.");
        }

        // რეგისტრაციის მეთოდი (წარმატებული რეგისტრაციის შემდეგ მომხმარებელი ავტორიზდება)
        public async Task<User> RegisterUserAsync(RegistrationDto registrationDto)
        {
            var user = new User
            {
                UserName = registrationDto.Username,
                Email = registrationDto.Email,
                FullName = registrationDto.FullName
            };

            var result = await _userManager.CreateAsync(user, registrationDto.Password);

            if (result.Succeeded)
            {
                // პაროლით ავტორიზაცია
                await _signInManager.SignInAsync(user, isPersistent: false);  // მომხმარებელი ავტომატურად ლოგინდება
                return user;
            }
            else
            {
                throw new Exception("Registration failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }
        }

        // ლოგინი (პაროლით)
        public async Task<bool> SignInUserAsync(LoginDto loginDto)
        {
            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user != null)
            {
                // პაროლის შემოწმება
                var result = await _signInManager.PasswordSignInAsync(user, loginDto.Password, false, false);

                return result.Succeeded;  // თუ პაროლი სწორია, წარმატებული ლოგინია
            }

            return false;  // არასწორი მონაცემები
        }

        // გამოსვლა
        public async Task SignOutUserAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}
