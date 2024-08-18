using ecommerceSharedLibrary.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProductAPIApplication.Interfaces;
using ProductAPIInfraestructure.Data;
using ProductAPIInfraestructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductAPIInfraestructure.DependencyInjection
{
    public static class ServiceContainer
    {
        public static IServiceCollection AddInfrastructureService(this IServiceCollection services, IConfiguration config)
        {
            // Add database connectivity
            // Add authentication scheme
            SharedServiceContainer.AddSharedServices<ProductDbContext>(services, config, config["MySerilog:FileName"]!);

            // Create dependency Injection (DI)
            services.AddScoped<IProduct, ProductRepository>();

            return services;
        }
        
        public static IApplicationBuilder UseInfrastructurePolicy(this IApplicationBuilder app)
        {
            // Register middleware such as:
            // Global Exception: handles external errors
            // Listen to Only Api Gateway: blocks all outsider call;
            SharedServiceContainer.UseSharedPolices(app);

            return app;
        }

    }
}
