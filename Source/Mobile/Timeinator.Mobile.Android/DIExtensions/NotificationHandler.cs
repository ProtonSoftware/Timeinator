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
        #region Private Static Constants

        private static readonly int NOTIFICATION_ID = 3030;
        private static readonly int ICON = Resource.Mipmap.logo;
        private static readonly string CHANNEL_ID = "session_notification";

        #endregion

        #region Private Properties

        private Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder { get; set; }
        private NotificationManager NManager => Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Type of the notificitaion
        /// </summary>
        public NotificationType Type { get; private set; }

        /// <summary>
        /// Shows last built Notification
        /// </summary>
        public void Notify()
        {
            if (NotificationBuilder == null)
                return;
            NManager.Notify(NOTIFICATION_ID, NotificationBuilder.Build());
        }

        /// <summary>
        /// Removes notification
        /// </summary>
        public void Cancel()
        {
            NManager.Cancel(NOTIFICATION_ID);
        }

        /// <summary>
        /// Builds notification which can be notified and updated
        /// </summary>
        public void BuildNotification(string title, string content, NotificationType type, AppAction action)
        {
            Type = type;
            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context, CHANNEL_ID)
                            .SetContentIntent(GetPendingIndent(action))
                            .SetSmallIcon(ICON)
                            .SetContentTitle(title)
                            .SetContentText(content);
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) // Deprecated since API 26
                builder.SetDefaults((int)NotificationDefaults.Lights);
            if (type == NotificationType.Progress)
            {
                builder.SetCategory(Notification.CategoryProgress);
                if (Build.VERSION.SdkInt >= BuildVersionCodes.O) // Only since API 26
                {
                    builder.SetVibrate(null);
                    builder.SetSound(null);
                }
            }
            else if (type == NotificationType.Prompt)
            {
                if (Build.VERSION.SdkInt < BuildVersionCodes.O) // Deprecated since API 26
                    builder.SetDefaults((int)NotificationDefaults.All);
            }
            NotificationBuilder = builder;
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(string title, string content)
        {
            if (NotificationBuilder == null)
                return;
            NotificationBuilder.SetContentTitle(title);
            NotificationBuilder.SetContentText(content);
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(int progress)
        {
            if (NotificationBuilder == null)
                return;
            NotificationBuilder.SetProgress(100, progress, false);
        }

        /// <summary>
        /// Change Notification's parameters
        /// </summary>
        public void UpdateNotification(string title, AppAction option)
        {
            if (NotificationBuilder == null)
                return;
            NotificationBuilder.AddAction(Resource.Drawable.abc_btn_radio_material, title, GetPendingIndent(option));
        }

        /// <summary>
        /// Creates standard Timeinator notification channel
        /// </summary>
        public void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) // required only since API 26
                return;
            if (NManager.GetNotificationChannel(CHANNEL_ID) != null) // channel already created
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

        private PendingIntent GetPendingIndent(AppAction action)
        {
            var intent = new Intent(Application.Context, typeof(ActionActivity));
            intent.SetAction(IntentActions.FromEnum(action));
            intent.PutExtra("NID", NOTIFICATION_ID);
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);
            return pendingIntent;
        }

        private Bitmap DecodeResource(int r) => BitmapFactory.DecodeResource(Application.Context.Resources, r);

        #endregion
    }
}