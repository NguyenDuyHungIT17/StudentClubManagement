using Microsoft.Extensions.DependencyInjection;
using StudentClub.Application.Services;

namespace StudentClub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
          
            services.AddScoped<AuthService, AuthService>();
            services.AddScoped<UserService, UserService>();
            services.AddScoped<ClubService, ClubService>();
            services.AddScoped<ClubMemberService, ClubMemberService>();
            services.AddScoped<InterviewService, InterviewService>();
            return services;
        }
    }
}
