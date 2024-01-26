using Kavior.Web.Models;
using Kavior.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Kavior.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _client;
        public BaseService(IHttpClientFactory client)
        {
            _client = client;
        }
        public async Task<ResponseDto>? SendAsync(RequestDto requestDto)
        {
            HttpClient client = _client.CreateClient("KaviorAPI");
            HttpRequestMessage message = new();
            message.Headers.Add("Accept", "application/json");
            //token for authen in futures

            // url: chỉ định URL mà chúng tôi sẽ gọi để truy cập bất kỳ API nào
            message.RequestUri = new Uri(requestDto.Url.ToString());

            if(requestDto.Data != null) {
                message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data),Encoding.UTF8,"application/json");
            }

            // after send request we receive a response
            HttpResponseMessage? apiResponse = null;

            switch(requestDto.ApiType)
            {
                case Utility.SD.ApiType.POST: 
                    message.Method = HttpMethod.Post;   break;
                case Utility.SD.ApiType.PUT:
                    message.Method = HttpMethod.Post; break;
                case Utility.SD.ApiType.DELETE:
                    message.Method = HttpMethod.Delete; break;
                default:
                    message.Method = HttpMethod.Get; break;
            }

            apiResponse = await client.SendAsync(message);

            try
            {
                switch (apiResponse.StatusCode)
                {
                    case HttpStatusCode.NotFound:
                        return new() { IsSuccess = false, Message = "Not Found" };
                    case HttpStatusCode.Forbidden:
                        return new() { IsSuccess = false, Message = "Access Denied" };
                    case HttpStatusCode.Unauthorized:
                        return new() { IsSuccess = false, Message = "Unauthorized" };
                    case HttpStatusCode.InternalServerError:
                        return new() { IsSuccess = false, Message = "Internal Server Error" };
                    default:
                        var apiContent = await apiResponse.Content.ReadAsStringAsync();
                        var apiResponseDto = JsonConvert.DeserializeObject<ResponseDto>(apiContent);
                        return apiResponseDto;
                }
            }
            catch (Exception ex)
            {
                var dto = new ResponseDto
                {
                    Message = ex.Message.ToString(),
                    IsSuccess = false
                };
                return dto;
            }
        }
    }
}
