namespace Jewelry_API.Constant;

public static class ApiEndPointConstant
{
    public const string RootEndPoint = "/api";
    public const string ApiVersion = "/v1";
    public const string ApiEndpoint = RootEndPoint + ApiVersion;
    
    public static class Account
    {
        public const string AccountEndPoint = ApiEndpoint + "/account";
        public const string RegisterAccount    = AccountEndPoint;
        public const string GetAccounts    = AccountEndPoint;
        public const string GetAccount    = AccountEndPoint + "/{id}";
        public const string UpdateAccount    = AccountEndPoint;
        public const string DeleteAccount    = AccountEndPoint + "/{id}";
    }
    
    public static class Authentication
    {
        public const string AuthenticationEndPoint = ApiEndpoint + "/auth";
        public const string ExchangeToken = AuthenticationEndPoint + "/token";
        public const string UserInfo = AuthenticationEndPoint + "/userinfo";
    }
    
    public static class Product
    {
        public const string ProductEndPoint = ApiEndpoint + "/product";
        public const string CreateProduct = ProductEndPoint;
        public const string GetAllProduct = ProductEndPoint;
        public const string GetProductById = ProductEndPoint + "/{id}";
        public const string UpdateProduct = ProductEndPoint + "/{id}";
        public const string DeleteProduct = ProductEndPoint + "/{id}";
        public const string CreateReview = ProductEndPoint + "/{id}/review";
        public const string GetAllReview = ProductEndPoint + "/{id}/review";
       
    }
    
    public static class Size
    {
        public const string SizeEndPoint = ApiEndpoint + "/size";
        public const string CreateSize = SizeEndPoint;
        public const string GetSizes = SizeEndPoint;
        public const string GetSize = SizeEndPoint + "/{id}";
    }
    
    public static class ProductSize
    {
        public const string ProductSizeEndPoint = Product.ProductEndPoint + "/{productId}" + "/sizes";
        public const string GetProductSizes = ProductSizeEndPoint;
        public const string CreateProductSize = ProductSizeEndPoint;
        public const string DeleteProductSize = ProductSizeEndPoint + "/{id}";
        public const string UpdateProductSize = ProductSizeEndPoint + "/{id}";
    }

    public static class Review
    {
        public const string ReviewEndPoint = ApiEndpoint + "/review";
        public const string DeleteReview = ReviewEndPoint + "/{id}";
    }
}