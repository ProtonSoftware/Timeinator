using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Timeinator.Mobile.Core
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
        public void BuildNotification(string title, string content, NotificationType type, AppAction action)
        {
            Type = type;
        }
        public void UpdateNotification(string title, string content)
        {
        }
        public void UpdateNotification(int progress)
        {
        }
        public void UpdateNotification(string title, AppAction option)
        {
        }
        public void CreateNotificationChannel()
        {
        }

        #endregion
    }
}