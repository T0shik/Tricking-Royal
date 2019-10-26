using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using IdentityServer.Configuration;
using Microsoft.Extensions.Options;

namespace IdentityServer.Services
{
    public class AuthMessageSender : IEmailSender
    {
        private readonly ILogger<AuthMessageSender> _logger;
        private readonly SmtpClient _client;
        private readonly EmailSettings _emailSettings;

        public AuthMessageSender(
            IOptions<EmailSettings> emailOptions,
            ILogger<AuthMessageSender> logger)
        {
            _emailSettings = emailOptions.Value;
            _logger = logger;
            _client = new SmtpClient(_emailSettings.Server)
            {
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_emailSettings.Username, _emailSettings.Password),
            };
        }

        public Task SendEmailAsync(string email, string subject, string message)
        {
            try
            {
                var mailMessage = new MailMessage(_emailSettings.Name, email, subject, message)
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