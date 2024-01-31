using Kavior.Web.Service.IService;
using Kavior.Web.Utility;

namespace Kavior.Web.Service
{
    public class TokenProvider : ITokenProvider
    {
        // when we are working with the cookies, we need to inject the HTTP context accessor.
        private readonly IHttpContextAccessor _contextAccessor;
        public TokenProvider(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }
        public void ClearToken()
        {
            _contextAccessor.HttpContext?.Response.Cookies.Delete(SD.TokenCookie);
        }

        public string? GetToken()
        {
            string? token = null;   
            bool? hasToken = (_contextAccessor.HttpContext?.Request.Cookies.TryGetValue(SD.TokenCookie, out token));
            return hasToken is true ? token : null;
        }

        public void SetToken(string token)
        {
            _contextAccessor.HttpContext?.Response.Cookies.Append(SD.TokenCookie, token);
        }
    }
}
