using Battles.Enums;

namespace Battles.Models
{
    public class NotificationConfiguration
    {
        public string Id { get; set; }
        public string UserInformationId { get; set; }
        public string NotificationId { get; set; }
        public NotificationConfigurationType ConfigurationType { get; set; }
        public bool Active { get; set; }
    }
}