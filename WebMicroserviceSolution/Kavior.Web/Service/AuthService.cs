using Kavior.Web.Models;
using Kavior.Web.Service.IService;
using Kavior.Web.Utility;
using System.Reflection;

namespace Kavior.Web.Service
{
    public class AuthService : IAuthService
    {

        public readonly IBaseService _baseService;
        public AuthService(IBaseService baseService)
        {
            _baseService = baseService;
        }

        public async Task<ResponseDto?> AssignRoleAsync(RegisterationRequestDto model)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = model,
                Url = SD.AuthAPIBase + "/api/auth/assignRole"
            });
        }

        public async Task<ResponseDto?> LoginAsync(LoginRequestDto loginRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = loginRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/login"
            }, withBear: false);
        }

        public async Task<ResponseDto?> RegisterAsync(RegisterationRequestDto registerationRequestDto)
        {
            return await _baseService.SendAsync(new RequestDto
            {
                ApiType = SD.ApiType.POST,
                Data = registerationRequestDto,
                Url = SD.AuthAPIBase + "/api/auth/register"
            }, withBear: false);
        }
    }
}
