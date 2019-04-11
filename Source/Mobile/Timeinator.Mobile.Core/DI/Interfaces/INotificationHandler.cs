using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile.Core
{
    public interface INotificationHandler
    {
        NotificationType Type { get; }
        void Notify();
        void Cancel();
        void BuildNotification(string title, string content, NotificationType type, AppAction action);
        void UpdateNotification(string title, string content);
        void UpdateNotification(int progress);
        void UpdateNotification(string title, AppAction option);
        void CreateNotificationChannel();
    }
}
