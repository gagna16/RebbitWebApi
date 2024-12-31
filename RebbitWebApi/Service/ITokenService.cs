using RebbitWebApi.Model;

namespace RebbitWebApi.Service
{
    
        public interface ITokenService
        {
            string GenerateJwtToken(User user);
        }
    
}
