using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Battles.Api.Workers.Notifications.Settings;
using Battles.Extensions;
using Battles.Models;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Battles.Api.Workers.Notifications.Dispatchers
{
    public class OneSignalDispatcher : IDispatcher
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<OneSignalDispatcher> _logger;
        private readonly OneSignal _config;

        public OneSignalDispatcher(
            IHttpClientFactory httpClientFactory,
            IOptions<OneSignal> config,
            ILogger<OneSignalDispatcher> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
            _config = config.Value;
        }

        public async Task SendNotification(
            NotificationMessage message,
            string targets,
            CancellationToken cancellationToken)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "https://onesignal.com/api/v1/notifications");

            request.Headers.Add("Authorization", $"Basic {_config.ApiKey}");

            var content = JsonConvert.SerializeObject(new
            {
                app_id = _config.AppId,
                headings = new {en = "Tricking Royal"},
                contents = new {en = message.Message},
                data = new
                {
                    id = message.Id,
                    navigation = message.Navigation.DefaultSplit(),
                    type = message.Type.ToString(),
                },
                include_player_ids = new[] {targets}
            });

            request.Content = new StringContent(content, Encoding.UTF8, "application/json");

            var response = await _httpClientFactory
                .CreateClient()
                .SendAsync(request, cancellationToken);

            var responseContent = await response.Content.ReadAsStringAsync();
            _logger.LogInformation(
                $"Notification request response status {response.StatusCode}, message: {responseContent}");
        }
    }
}