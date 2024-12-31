using RebbitWebApi.Dto;
using RebbitWebApi.Model;

namespace RebbitWebApi.Service
{
    public interface IUserService
    {
        Task<User> RegisterUserAsync(RegistrationDto registrationDto);
        Task<string> LoginUserAsync(LoginDto loginDto);
        Task<string> GenerateJwtToken(User user);
        Task<List<User>> GetUser();
    }
}
