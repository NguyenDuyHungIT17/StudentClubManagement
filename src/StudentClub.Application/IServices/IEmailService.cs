namespace StudentClub.Application.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> values);
        Task SendInterviewResultEmailAsync(int clubId, int resultType);
        
    }
}
