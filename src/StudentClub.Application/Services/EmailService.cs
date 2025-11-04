using Microsoft.Extensions.Configuration;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace StudentClub.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IInterviewRepository _interviewRepository;
        private readonly ILogger<EmailService> _logger;

        private readonly IConfiguration _config;
        private readonly string _templatePath;
        private readonly string _from;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;

        public EmailService(IConfiguration config, IClubRepository clubRepository, IInterviewRepository interviewRepository, ILogger<EmailService> logger)
        {
            _config = config;
            _templatePath = Path.Combine(AppContext.BaseDirectory, "EmailTemplates");
            _from = _config["Smtp:From"];
            _host = _config["Smtp:Host"];
            _port = int.Parse(_config["Smtp:Port"]);
            _user = _config["Smtp:Username"];
            _pass = _config["Smtp:Password"];

            _clubRepository = clubRepository;
            _interviewRepository = interviewRepository;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
        {
            try
            {
                using var client = new SmtpClient(_host, _port)
                {
                    Credentials = new NetworkCredential(_user, _pass),
                    EnableSsl = true
                };
                var mail = new MailMessage(_from, to, subject, htmlBody)
                {
                    IsBodyHtml = true
                };
                await client.SendMailAsync(mail);
            }
            catch (Exception ex)
            {
                _logger.LogError("Có lỗi khi gửi email ");
                throw;
            }
           
        }

        public async Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> values)
        {
            try
            {
                var filePath = Path.Combine(_templatePath, templateName);
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"Không tìm thấy template: {filePath}");

                var template = await File.ReadAllTextAsync(filePath);
                foreach (var pair in values)
                {
                    template = template.Replace("{{" + pair.Key + "}}", pair.Value);
                }
                return template;
            }
            catch (Exception ex)
            {
                _logger.LogError("Có lỗi khi render template email");
                throw;
            }
            
        }

        public async Task SendInterviewResultEmailAsync(int clubId, string resultType)
        {
            try
            {
                var club = await _clubRepository.GetClubByClubIdAsync(clubId);
                var interviews = await _interviewRepository.GetByClubIdAsync(clubId);

                string templateName = resultType == "Pass" ? "InterviewPass.html" : "InterviewFail.html";
                string subject = "Kết quả phỏng vấn CLB";

                foreach (var interview in interviews)
                {
                    if (interview.Result == resultType)
                    {
                        var html = await RenderTemplateAsync(templateName, new Dictionary<string, string>
                    {
                        { "ApplicantName", interview.ApplicantName },
                        { "ClubName", club.ClubName }
                    });
                        await SendEmailAsync(interview.ApplicantEmail, subject, html);
                    }
                }
            }catch(Exception ex)
            {
                _logger.LogError("Có lỗi khi gửi email thông báo");
                throw;
            }
            
        }
    }
}