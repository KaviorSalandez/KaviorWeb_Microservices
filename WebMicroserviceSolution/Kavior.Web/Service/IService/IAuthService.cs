using Kavior.Web.Models;

namespace Kavior.Web.Service.IService
{
    public interface IAuthService
    {
        Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto);
        Task<ResponseDto?> RegisterAsync(RegisterationRequestDto registerationRequestDto);

        Task<ResponseDto?> AssignRoleAsync(RegisterationRequestDto model);
    }
}
