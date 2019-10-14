using System;
using Battles.Enums;

namespace Battles.Models
{
    public class NotificationMessage
    {
        public int Id { get; set; }
        public string UserInformationId { get; set; }
        public string Message { get; set; }
        public string Navigation { get; set; }
        public NotificationMessageType Type { get; set; }
        public bool New { get; set; } = true;
        public DateTime TimeStamp { get; set; } = DateTime.UtcNow;
    }
}