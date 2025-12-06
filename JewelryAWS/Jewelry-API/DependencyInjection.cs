using Jewelry_Model.Entity;
using Jewelry_Repository.Implement;
using Jewelry_Repository.Interface;
using Jewelry_Service.Implements;
using Jewelry_Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Jewelry_API;

public static class DependencyInjection
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork<JewelryAwsContext>, UnitOfWork<JewelryAwsContext>>();
            return services;
        }
        public static IServiceCollection AddCustomServices(this IServiceCollection services)
        {
            services.AddScoped<ISizeService, SizeService>();
            services.AddScoped<IUploadService, UploadService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<IProductSizeService, ProductSizeService>();
            services.AddScoped<IReviewService, ReviewService>();
            return services;
        }
        public static IServiceCollection AddDatabase(this IServiceCollection services)
        {
            services.AddDbContext<JewelryAwsContext>(options => options.UseNpgsql(GetConnectionString()));
            return services;
        }
        private static string GetConnectionString()
        {
            IConfigurationRoot config = new ConfigurationBuilder()
                 .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", true, true)
                        .Build();
            var strConn = config["ConnectionStrings:DefaultDB"];

            return strConn;
        }

    }