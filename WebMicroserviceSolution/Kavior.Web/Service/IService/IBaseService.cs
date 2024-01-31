using Kavior.Web.Models;

namespace Kavior.Web.Service.IService
{
    public interface IBaseService
    {
       Task<ResponseDto>? SendAsync(RequestDto requestDto, bool withBear = true);
    }
}
