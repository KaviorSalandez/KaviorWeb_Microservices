﻿using Kavior.Web.Models;
using Kavior.Web.Service.IService;
using Newtonsoft.Json;
using System.Net;
using System.Text;

namespace Kavior.Web.Service
{
    public class BaseService : IBaseService
    {
        private readonly IHttpClientFactory _client;
        private readonly ITokenProvider _tokenProvider;
        public BaseService(IHttpClientFactory client, ITokenProvider tokenProvider)
        {
            _client = client;
            _tokenProvider = tokenProvider;
        }
        public async Task<ResponseDto>? SendAsync(RequestDto requestDto, bool withBear = true)
        {
            HttpClient client = _client.CreateClient("KaviorAPI");
            HttpRequestMessage message = new();
            if(requestDto.ContentType==Utility.SD.ContentType.MultipartFormData)
            {
                message.Headers.Add("Accept", "*/*");
            }
            else
            {
                message.Headers.Add("Accept", "application/json");
            }


            //token for authen in futures
            if (withBear)
            {
                var token = _tokenProvider.GetToken();
                message.Headers.Add("Authorization", $"Bearer {token}");
            }

            // url: chỉ định URL mà chúng tôi sẽ gọi để truy cập bất kỳ API nào
            message.RequestUri = new Uri(requestDto.Url.ToString());

            if(requestDto.ContentType == Utility.SD.ContentType.MultipartFormData)
            {
                var content = new MultipartFormDataContent();
                foreach( var prop in requestDto.Data.GetType().GetProperties() )
                {
                    var value = prop.GetValue(requestDto.Data);
                    if (value is FormFile)
                    {
                        var file = (FormFile)value;
                        if (file != null)
                        {
                            content.Add(new StreamContent(file.OpenReadStream()), prop.Name,file.FileName);
                        }
                    }
                    else
                    {
                        content.Add(new StringContent(value == null ? "" : value.ToString()), prop.Name);
                        //Nếu prop.Name là "Name" và value là "John", thì content.Add(new StringContent("John"), "Name") sẽ thêm "John" vào phần tử "Name" của MultipartFormDataContent.
                    }
                }
                message.Content = content;
            }
            else
            {
                if (requestDto.Data != null)
                {
                    message.Content = new StringContent(JsonConvert.SerializeObject(requestDto.Data), Encoding.UTF8, "application/json");
                }
            }

           

            // after send request we receive a response
            HttpResponseMessage? apiResponse = null;

            switch(requestDto.ApiType)
            {
                case Utility.SD.ApiType.POST: 
                    message.Method = HttpMethod.Post;   break;
                case Utility.SD.ApiType.PUT:
                    message.Method = HttpMethod.Put; break;
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
