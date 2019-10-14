using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading;
using System.Threading.Tasks;
using Battles.Configuration;
using Battles.Enums;
using Battles.Extensions;
using Battles.Models;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;

namespace Battles.Api.Notifications.Dispatchers
{
    public class EmailDispatcher : IDispatcher
    {
        private readonly EmailSettings _emailSettings;
        private readonly Routing _routing;
        private readonly ILogger<EmailDispatcher> _logger;
        private readonly SmtpClient _smtp;

        public EmailDispatcher(
            EmailSettings emailSettings,
            Routing routing,
            ILogger<EmailDispatcher> logger)
        {
            _emailSettings = emailSettings;
            _routing = routing;
            _logger = logger;
            _smtp = new SmtpClient(emailSettings.Server)
            {
                Credentials = new NetworkCredential(emailSettings.Username, emailSettings.Password),
            };
        }

        public Task SendNotification(
            NotificationMessage message,
            string target,
            CancellationToken cancellationToken)
        {
            var navigation = CreateEmailNavigation(message);
            var htmlMessage = $@"
<h4>Notification from {_emailSettings.Name}</h4>
<p>{message.Message}</p>
<hr />
<p>
    Follow the <a href={navigation}>link</a> to see the update.
</p>";

            var mailMessage = new MailMessage(
                _emailSettings.Name,
                target,
                message.Message,
                htmlMessage)
            {
                IsBodyHtml = true,
            };

            return _smtp.SendMailAsync(mailMessage);
        }

        //This will have a strong link with how the routing works in the client app.
        private string CreateEmailNavigation(NotificationMessage message)
        {
            var navigation = message.Navigation.DefaultSplit();
            switch (message.Type)
            {
                case NotificationMessageType.Empty:
                    const string msg = "Tried to send email to notification type empty";
                    _logger.LogError(msg);
                    throw new Exception(msg);
                case NotificationMessageType.MatchHistory:
                    return CreateRoute("/battles/history", new Dictionary<string, string>
                    {
                        ["matchId"] = navigation[0],
                    });
                case NotificationMessageType.MatchActive:
                    return CreateRoute("/battles/active", new Dictionary<string, string>
                    {
                        ["matchId"] = navigation[0],
                    });
                case NotificationMessageType.Comment:
                    return CreateRoute($"/watch/{navigation[0]}", new Dictionary<string, string>
                    {
                        ["comment"] = navigation[1],
                    });
                case NotificationMessageType.SubComment:
                    return CreateRoute($"/watch/{navigation[0]}", new Dictionary<string, string>
                    {
                        ["comment"] = navigation[1],
                        ["subComment"] = navigation[2],
                    });
                case NotificationMessageType.TribunalHistory:
                    return CreateRoute("/tribunal/history", new Dictionary<string, string>
                    {
                        ["matchId"] = navigation[0],
                    });
                case NotificationMessageType.TribunalFlag:
                    return CreateRoute("/tribunal/flag", new Dictionary<string, string>
                    {
                        ["matchId"] = navigation[0],
                    });
                case NotificationMessageType.Friend:
                    break;
                case NotificationMessageType.Invitation:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(message.Type), message.Type,
                        "This message type doesn't exist.");
            }

            return "";
        }

        private string CreateRoute(string route, IDictionary<string, string> queryParams)
        {
            var uri = $"{_routing.Client}/{route}";
            return QueryHelpers.AddQueryString(uri, queryParams);
        }
    }
}