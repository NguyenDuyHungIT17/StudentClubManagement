using Microsoft.Extensions.DependencyInjection;
using StudentClub.Application.Services;

namespace StudentClub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
          
            services.AddScoped<AuthService, AuthService>();

            return services;
        }
    }
}
