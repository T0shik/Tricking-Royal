using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace IdentityServer.Services
{
    public class AuthMessageSender : IEmailSender
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AuthMessageSender> _logger;

        public AuthMessageSender(
            IConfiguration config,
            ILogger<AuthMessageSender> logger)
        {
            _config = config;
            _logger = logger;
        }

        public async Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var client = new SmtpClient(_config["Email:SMTP"])
                {
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential(_config["Email:Username"], _config["Email:Password"])
                };
                var mailMessage = new MailMessage
                {
                    To = { email },
                    Subject = subject,
                    Body = message,
                    From = new MailAddress(_config["Email:Username"]),
                    IsBodyHtml = true
                };
                await client.SendMailAsync(mailMessage);
                return;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return;
            }
        }

        public Task SendSmsAsync(string number, string message)
        {
            // Plug in your SMS service here to send a text message.
            return Task.FromResult(0);
        }
    }
}
