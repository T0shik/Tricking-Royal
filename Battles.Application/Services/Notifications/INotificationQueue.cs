using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Battles.Enums;

namespace Battles.Application.Services.Notifications
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

        Task Pop(CancellationToken cancellationToken);
    }
}