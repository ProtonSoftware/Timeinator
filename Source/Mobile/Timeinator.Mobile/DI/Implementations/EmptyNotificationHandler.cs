using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Notification handling for unsupporting platforms
    /// </summary>
    public class EmptyNotificationHandler : INotificationHandler
    {
        #region Interface Implementation

        public NotificationType Type { get; private set; }
        public void Notify()
        {
        }
        public void Cancel()
        {
        }
        public void BuildNotification(string title, string content, NotificationType type, NotificationAction action)
        {
            Type = type;
        }
        public void UpdateNotification(string title, string content)
        {
        }
        public void UpdateNotification(int progress)
        {
        }
        public void UpdateNotification(string title, NotificationAction option)
        {
        }
        public void CreateNotificationChannel()
        {
        }

        #endregion
    }
}