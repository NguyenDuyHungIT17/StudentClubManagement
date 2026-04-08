using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentClub.Application.Interfaces;
using StudentClub.Infrastructure.Persistence;
using StudentClub.Infrastructure.Realtime;
using StudentClub.Infrastructure.Repositories;
using StudentClub.Infrastructure.Utils;

namespace StudentClub.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<StudentClubDbContext>(options =>
                options.UseSqlServer(
                    configuration.GetConnectionString("DefaultConnection"),
                    b => b.MigrationsAssembly(typeof(StudentClubDbContext).Assembly.FullName)
                )
            );

            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IClubRepository, ClubRepository>();
            services.AddScoped<IClubMemberRepository, CLubMemberRepository>();

            services.AddScoped<IEventRepository, EventRepository>();
            services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();
            services.AddScoped<IInterviewRepository, InterviewRepository>();
            services.AddScoped<IFeedbackRepository, FeedbackRepository>();

            services.AddScoped<ICampaignRepository, CampaignRepository>();
            services.AddScoped<IPhotoRepository, PhotoRepository>();

            services.AddScoped<IChatRepository, ChatRepository>();
            services.AddSingleton<IRealtimeConnectionManager, WebSocketConnectionManager>();

            return services;
        }
    }
}