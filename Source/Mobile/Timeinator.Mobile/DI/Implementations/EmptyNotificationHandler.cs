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
        #region Interface implementation

        /// <summary>
        /// Shows last built Notification
        /// </summary>
        public void Notify()
        {
        }

        public void Cancel()
        {
        }

        /// <summary>
        /// Builds notification which can be notified and updated
        /// </summary>
        public void BuildNotification(string title, string content, NotificationType type, NotificationAction action)
        {
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(string title, string content)
        {
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(int progress)
        {
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(string title, NotificationAction option)
        {
        }

        /// <summary>
        /// Creates standard Timeinator notification channel
        /// </summary>
        public void CreateNotificationChannel()
        {
        }

        #endregion
    }
}