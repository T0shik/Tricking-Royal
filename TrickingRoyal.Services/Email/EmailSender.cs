using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace TrickingRoyal.Services.Email
{
    public class EmailSender : IEmailSender
    {
        private readonly ILogger<EmailSender> _logger;
        private readonly SmtpClient _client;
        private readonly EmailSettings _emailSettings;

        public EmailSender(
            EmailSettings emailOptions,
            ILogger<EmailSender> logger)
        {
            _emailSettings = emailOptions;
            _logger = logger;
            _client = new SmtpClient(_emailSettings.Server)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
            };
        }

        public Task SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                var mailMessage = new MailMessage(_emailSettings.Name, to, subject, body)
                {
                    IsBodyHtml = true,
                };

                return _client.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }

            return Task.CompletedTask;
        }
    }
}