using System;
using System.Linq.Expressions;
using Battles.Extensions;
using Battles.Models;

namespace Battles.Application.ViewModels
{
    public class NotificationsViewModel
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string TimeStamp { get; set; }
        public string Type { get; set; }
        public string[] Navigation { get; set; }
        public bool New { get; set; }

        public static Expression<Func<NotificationMessage, NotificationsViewModel>> Projection =
            notification => new NotificationsViewModel()
            {
                Id = notification.Id,
                Message = notification.Message,
                TimeStamp = notification.TimeStamp.ToString("dd-MM-yyyy hh:mm"),
                Type = notification.Type.ToString(),
                Navigation = notification.Navigation.DefaultSplit(),
                New = notification.New,
            };
    }
}