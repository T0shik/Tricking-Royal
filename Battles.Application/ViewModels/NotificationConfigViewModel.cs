using System;
using System.Linq.Expressions;
using Battles.Models;

namespace Battles.Application.ViewModels
{
    public class NotificationConfigViewModel
    {
        public bool Enabled { get; set; }
        public string NotificationId { get; set; }
        public int Type { get; set; }

        public static Expression<Func<NotificationConfiguration, NotificationConfigViewModel>> Projection =
            config => new NotificationConfigViewModel
            {
                Enabled = config.Active,
                NotificationId = config.NotificationId,
                Type = (int) config.ConfigurationType,
            };
    }
}