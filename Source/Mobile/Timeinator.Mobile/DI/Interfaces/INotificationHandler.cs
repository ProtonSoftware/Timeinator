using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile
{
    public interface INotificationHandler
    {
        void Notify();
        void BuildNotification(NotificationType type, string title, string content, NotificationAction action, object extra = null);
        void CreateNotificationChannel();
    }
}
