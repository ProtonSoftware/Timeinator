using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;

namespace Timeinator.Mobile.Droid
{
    /// <summary>
    /// Notification handling for Android
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        #region Private Memebers

        private static readonly int NOTIFICATION_ID = 3030;
        private static readonly int ICON = Resource.Mipmap.logo;
        private static readonly string CHANNEL_ID = "session_notification";
        private Android.Support.V4.App.NotificationCompat.Builder mNotificationBuilder;
        private NotificationType mType;
        private NotificationManager NManager => Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        #endregion

        #region Interface implementation

        /// <summary>
        /// Shows last built Notification
        /// </summary>
        public void Notify()
        {
            if (mNotificationBuilder == null)
                return;
            NManager.Notify(NOTIFICATION_ID, mNotificationBuilder.Build());
        }

        public void Cancel()
        {
            NManager.Cancel(NOTIFICATION_ID);
        }

        /// <summary>
        /// Builds notification which can be notified and updated
        /// </summary>
        public void BuildNotification(string title, string content, NotificationType type, NotificationAction action)
        {
            mType = type;
            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context, CHANNEL_ID)
                            .SetContentIntent(GetPendingIndent(action))
                            .SetSmallIcon(ICON)
                            .SetContentTitle(title)
                            .SetContentText(content);
            if (type == NotificationType.Progress)
                builder.SetCategory(Notification.CategoryProgress);
            else if (type == NotificationType.Prompt)
            {
                builder.SetCategory(Notification.CategoryTransport);
                builder.SetDefaults((int)NotificationDefaults.All);
            }
            else
            {
                builder.SetCategory(Notification.CategoryService);
                builder.SetDefaults((int)NotificationDefaults.All);
            }
            mNotificationBuilder = builder;
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(string title, string content)
        {
            if (mNotificationBuilder == null)
                return;
            mNotificationBuilder.SetContentTitle(title);
            mNotificationBuilder.SetContentText(content);
            Notify();
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(int progress)
        {
            if (mNotificationBuilder == null)
                return;
            mNotificationBuilder.SetProgress(100, progress, true);
            Notify();
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(string title, NotificationAction option)
        {
            if (mNotificationBuilder == null)
                return;
            mNotificationBuilder.AddAction(Resource.Drawable.navigation_empty_icon, title, GetPendingIndent(option));
            Notify();
        }

        /// <summary>
        /// Creates standard Timeinator notification channel
        /// </summary>
        public void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) // required only since API 26
                return;
            if (NManager.GetNotificationChannel(CHANNEL_ID) == null) // channel already created
                return;
            var channelName = "Timeinator";
            var channelDescription = "Timeinator session notifications";
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.High)
            {
                Description = channelDescription,
                LockscreenVisibility = NotificationVisibility.Public
            };
            NManager.CreateNotificationChannel(channel);
        }

        #endregion

        #region Private Helpers

        private PendingIntent GetPendingIndent(NotificationAction action)
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.PutExtra("AID", (int)action);
            intent.PutExtra("NID", NOTIFICATION_ID);
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);
            return pendingIntent;
        }

        private Bitmap DecodeResource(int r) => BitmapFactory.DecodeResource(Application.Context.Resources, r);

        #endregion
    }
}