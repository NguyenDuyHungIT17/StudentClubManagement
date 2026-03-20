
using Microsoft.Extensions.DependencyInjection;

using StudentClub.Application.IServices;
using StudentClub.Application.IServices.IRealtimeService;
using StudentClub.Application.Mapper;
using StudentClub.Application.Services;
using StudentClub.Application.Services.RealtimeServices;

namespace StudentClub.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
          
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IClubMemberService, ClubMemberService>();
            services.AddScoped<IInterviewService, InterviewService>();
            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IEventRegistrationService, EventRegistrationService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddSingleton<IRealtimeService, ChatService>();
            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<EventMapping>();
            services.AddScoped<EventRegistrationMapping>();
            services.AddScoped<FeedbackMapping>();
            services.AddScoped<ClubMemberMapping>();
            services.AddScoped<InterviewMapping>();
            return services;
        }
    }
}
