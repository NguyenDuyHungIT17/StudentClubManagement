using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentClub.Application.IServices
{
    public interface IEmailService
    {
        Task SendEmailAsync(string to, string subject, string htmlBody);
        Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> values);
        Task SendInterviewResultEmailAsync(int clubId, string resultType); // resultType: "Pass" hoặc "Fail"

        
    }
}
