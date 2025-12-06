using Jewelry_Model.Entity;
using Jewelry_Repository.Implement;
using Jewelry_Repository.Interface;
using Microsoft.EntityFrameworkCore;

namespace Jewelry_API;

public static class DependencyInjection
    {
        public static IServiceCollection AddUnitOfWork(this IServiceCollection services)
        {
            services.AddScoped<IUnitOfWork<JewelryAwsContext>, UnitOfWork<JewelryAwsContext>>();
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