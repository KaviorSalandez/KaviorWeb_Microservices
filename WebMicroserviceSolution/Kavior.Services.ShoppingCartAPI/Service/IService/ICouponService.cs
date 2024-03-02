using Kavior.Services.ShoppingCartAPI.Models.Dto;

namespace Kavior.Services.ShoppingCartAPI.Service.IService
{
    public interface ICouponService
    {
        Task<CouponDto> GetCouponDto(string couponCode);    
    }
}
