using Kavior.Services.ShoppingCartAPI.Models.Dto;
using Kavior.Services.ShoppingCartAPI.Service.IService;
using Newtonsoft.Json;

namespace Kavior.Services.ShoppingCartAPI.Service
{
    public class ProductService : IProductService
    {
        public readonly IHttpClientFactory _httpClientFactory;
        public ProductService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        public async Task<IEnumerable<ProductDto>> GetProducts()
        {
            var client = _httpClientFactory.CreateClient("Product");
            var response = await client.GetAsync($"/api/product");
            var apiContent = await response.Content.ReadAsStringAsync();    
            var res = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
            if (res.IsSuccess)
            {
                return JsonConvert.DeserializeObject<IEnumerable<ProductDto>>(Convert.ToString(res.Result));
            }
            return new List<ProductDto>();
        }
    }
}
