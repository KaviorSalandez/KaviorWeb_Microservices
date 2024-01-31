namespace Kavior.Web.Utility
{
    public class SD
    {
        // url base for api
        public static string CouponAPIBase { get;set; }
        public static string AuthAPIBase { get;set; }


        // role
        public const string RoleAdmin = "ADMIN";
        public const string RoleCustomer = "CUSTOMER";
        // token
        public const string TokenCookie = "JWTToken";

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE, 
        }
    }
}
