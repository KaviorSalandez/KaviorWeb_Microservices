
using Kavior.Service.OrderAPI.Models;

namespace Kavior.Services.OrderAPI.Service.IService
{
    public interface IProductService
    {
        Task<IEnumerable<ProductDto>> GetProducts();
    }
}
