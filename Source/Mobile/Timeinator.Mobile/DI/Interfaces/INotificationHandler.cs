using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile
{
    public interface INotificationHandler
    {
        void Notify();
        void Cancel();
        void BuildNotification(string title, string content, NotificationType type, NotificationAction action);
        void UpdateNotification(string title, string content);
        void UpdateNotification(int progress);
        void UpdateNotification(string title, NotificationAction option);
        void CreateNotificationChannel();
    }
}
