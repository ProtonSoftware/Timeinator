using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Timeinator.Mobile.Droid
{
    /// <summary>
    /// Notification handling for Android
    /// </summary>
    public class NotificationHandler : INotificationHandler
    {
        #region Private Memebers

        private static readonly int NOTIFICATION_ID = 3030;
        private static readonly string CHANNEL_ID = "session_notification";
        private Notification mReadyNotification;

        #endregion

        #region Interface implementation

        /// <summary>
        /// Shows last built Notification
        /// </summary>
        public void Notify()
        {
            //var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(this);
            if (mReadyNotification == null)
                return;
            var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(NOTIFICATION_ID, mReadyNotification);
        }

        /// <summary>
        /// Builds notification redirecting main activity
        /// </summary>
        /// <param name="extra">Pipeline for image or progress</param>
        public void BuildNotification(NotificationType type, string title, string content, NotificationAction action, object extra=null)
        {
            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.PutExtra("AID", (int)action);
            intent.PutExtra("NID", NOTIFICATION_ID);
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);
            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context, CHANNEL_ID)
                            .SetContentIntent(pendingIntent)
                            .SetContentTitle(title)
                            .SetDefaults((int)NotificationDefaults.All)
                            .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                            .SetContentText(content);
            if (type == NotificationType.Progress)
            {
                builder.SetCategory(Notification.CategoryProgress);
            }
            else if (type == NotificationType.Prompt)
            {
                builder.SetCategory(Notification.CategoryTransport);
            }
            else
            {
                builder.SetCategory(Notification.CategoryService);
            }
            mReadyNotification = builder.Build();
        }

        /// <summary>
        /// Creates standard Timeinator notification channel
        /// </summary>
        public void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) // required only since API 26
                return;
            var notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
            if (notificationManager.GetNotificationChannel(CHANNEL_ID) == null) // channel already created
                return;
            var channelName = "Timeinator";
            var channelDescription = "Timeinator session notifications";
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.High)
            {
                Description = channelDescription,
                LockscreenVisibility = NotificationVisibility.Public
            };
            notificationManager.CreateNotificationChannel(channel);
        }

        #endregion
    }
}