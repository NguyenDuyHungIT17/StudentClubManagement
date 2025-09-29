using MailKit;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentClub.Application.Interfaces;
using StudentClub.Infrastructure.Persistence;
using StudentClub.Infrastructure.Repositories;
using StudentClub.Infrastructure.Utils;

namespace StudentClub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StudentClubDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<IClubMemberRepository, CLubMemberRepository>();
            services.AddScoped<IInterviewRepository, InterviewRepository>();
            services.AddScoped<IEventRepository, EventRepository>();
            //services.Configure<MailSettings>(configuration.GetSection("MailSettings"));
            //services.AddSingleton<ITemplateRenderer, TemplateRenderer>();
            //services.AddTransient<IMailService, MailService>();
            return services;
        }
    }
}
