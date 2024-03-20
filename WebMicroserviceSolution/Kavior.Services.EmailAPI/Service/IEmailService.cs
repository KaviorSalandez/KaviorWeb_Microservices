using Kavior.Services.EmailAPI.Models.Dto;

namespace Kavior.Services.EmailAPI.Service
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);

    }
}
