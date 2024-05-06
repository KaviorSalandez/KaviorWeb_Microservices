using Kavior.Services.EmailAPI.Models.Dto;
using Kavior.Services.EmailAPI.Message;

namespace Kavior.Services.EmailAPI.Service
{
    public interface IEmailService
    {
        Task EmailCartAndLog(CartDto cartDto);

        Task RegisterUserEmailAndLog(string email);

        Task LogOrderPlaced(RewardsMessage rewardsMessage);
    }
}
