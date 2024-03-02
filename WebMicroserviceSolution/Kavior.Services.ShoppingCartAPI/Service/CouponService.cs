using Kavior.Services.ShoppingCartAPI.Models.Dto;
using Kavior.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Kavior.Services.ShoppingCartAPI.Service
{
    public class CouponService : ICouponService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public CouponService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<CouponDto> GetCouponDto(string couponCode)
        {
            var client = _httpClientFactory.CreateClient("Coupon");
            var response = await client.GetAsync($"/api/coupon/GetByCode/{couponCode}");
            var apiContent = await response.Content.ReadAsStringAsync();
            var res = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (res!=null && res.IsSuccess)
            {
                return JsonConvert.DeserializeObject<CouponDto>(Convert.ToString(res.Result));
            }
            return new CouponDto();
        }
    }
}
