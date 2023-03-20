using Microsoft.OpenApi.Models;

namespace BRN.identidade.API.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(u =>
            {
                u.SwaggerDoc(name: "v1", new OpenApiInfo
                {
                    Title = "pinky e cerebro: brain",
                    Description = "servidorzinho REST distibuido ecomerce",
                    Contact = new OpenApiContact() { Name = "Vinicius Pretto", Email = "pvinig@gmail.com" }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(u =>
            {
                u.SwaggerEndpoint(url: "/swagger/v1/swagger.json", name: "v1");
            });

            return app;
        }
    }
}
