using Microsoft.Extensions.DependencyInjection;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using StudentClub.Application.IServices.IRealtimeService;
using StudentClub.Application.Mapper;
using StudentClub.Application.Mappings;
using StudentClub.Application.Realtime;
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
            services.AddScoped<IUserContext, UserContext>();

            services.AddScoped<IClubService, ClubService>();
            services.AddScoped<IClubMemberService, ClubMemberService>();

            services.AddScoped<IEventService, EventService>();
            services.AddScoped<IEventRegistrationService, EventRegistrationService>();
            services.AddScoped<IInterviewService, InterviewService>();

            services.AddScoped<ICampaignService, CampaignService>();
            services.AddScoped<IPhotoService, PhotoService>();
            services.AddScoped<IFeedbackService, FeedbackService>();
            services.AddScoped<IEmailService, EmailService>();

            services.AddScoped<IChatService, ChatService>();

            services.AddScoped<IRealtimeService, RealtimeService>();
            services.AddScoped<IVnPayService, VnPayService>();
            services.AddScoped<ChatMapper>();
            services.AddScoped<CampaignMapping>();
            services.AddScoped<EventMapping>();
            services.AddScoped<EventRegistrationMapping>();
            services.AddScoped<FeedbackMapping>();
            services.AddScoped<ClubMemberMapping>();
            services.AddScoped<InterviewMapping>();
            services.AddScoped<PhotoMapper>();

            return services;
        }
    }
}