using BRN.WebApp.MVC.Services;
using Microsoft.AspNetCore.Http;
using BRN.WebApp.MVC.Extensions;


namespace BRN.WebApp.MVC.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static void RegisterServices(this IServiceCollection services)
        {
            services.AddHttpClient<IAuthenticationService, AuthenticationService>();

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddScoped<IUser, AspNetUser>();
        }
    }
}
