using Microsoft.Extensions.Configuration;
using StudentClub.Application.Interfaces;
using StudentClub.Application.IServices;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.IO;
using System.Threading.Tasks;

namespace StudentClub.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IClubRepository _clubRepository;
        private readonly IInterviewRepository _interviewRepository;

        private readonly IConfiguration _config;
        private readonly string _templatePath;
        private readonly string _from;
        private readonly string _host;
        private readonly int _port;
        private readonly string _user;
        private readonly string _pass;

        public EmailService(IConfiguration config, IClubRepository clubRepository, IInterviewRepository interviewRepository)
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
        }

        public async Task SendEmailAsync(string to, string subject, string htmlBody)
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

        public async Task<string> RenderTemplateAsync(string templateName, Dictionary<string, string> values)
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

        public async Task SendInterviewResultEmailAsync(int clubId, string resultType)
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
        }
    }
}