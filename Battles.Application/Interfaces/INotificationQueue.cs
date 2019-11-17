using System.Collections.Generic;
using Battles.Enums;

namespace Battles.Application.Interfaces
{
    public interface INotificationQueue
    {
        void QueueNotification(string message, string navigation, NotificationMessageType type,
            IEnumerable<string> targets);
    }
}