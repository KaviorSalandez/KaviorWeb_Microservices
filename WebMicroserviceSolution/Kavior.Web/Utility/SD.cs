namespace Kavior.Web.Utility
{
    public class SD
    {
        // url base for api
        public static string CouponAPIBase { get;set; }
        public static string AuthAPIBase { get;set; }
        public static string ProductAPIBase { get;set; }
        public static string ShoppingCartAPIBase { get;set; }
        public static string OrderAPIBase { get;set; }


        // role
        public const string RoleAdmin = "ADMIN";

        public const string RoleCustomer = "CUSTOMER";
        // token
        public const string TokenCookie = "JWTToken";
        //status
        public const string Status_Pending = "Pending";
        public const string Status_Approved = "Approved";
        public const string Status_ReadyForPickup = "ReadyForPickup";
        public const string Status_Complete = "Complete";
        public const string Status_Refund = "Refunded";
        public const string Status_Cancelled = "Cancelled";
        // content type
        public enum ContentType
        {
            Json,
            MultipartFormData,
        }

        public enum ApiType
        {
            GET,
            POST,
            PUT,
            DELETE, 
        }
    }
}
