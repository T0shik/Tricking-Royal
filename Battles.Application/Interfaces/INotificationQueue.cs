using System;
using System.Collections.Generic;
using Battles.Enums;
using Battles.Models;

namespace Battles.Application.Interfaces
{
    public interface INotificationQueue
    {
        void QueueNotification(
            string message,
            string navigation,
            NotificationMessageType type,
            IEnumerable<string> targets,
            bool force = false,
            bool save = true);
    }
}