using Kavior.Services.ShoppingCartAPI.Models.Dto;

namespace Kavior.Services.ShoppingCartAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
