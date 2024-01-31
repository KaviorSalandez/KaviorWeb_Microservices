using Kavior.Services.AuthAPI.Models;

namespace Kavior.Services.AuthAPI.Service.IService
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(ApplicationUser appUser, IEnumerable<string> roles);
    }
}
