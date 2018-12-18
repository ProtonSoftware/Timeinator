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
        private static readonly int NOTIFICATION_ID = 3030;
        private static readonly string CHANNEL_ID = "session_notification";

        /// <summary>
        /// Builds basic notification redirecting to session page and shows it
        /// </summary>
        public void BuildNotification(string title, string content)
        {
            //var intent = new Intent(this, typeof(MainActivity));
            //const int pendingIntentId = 0;
            //var pendingIntent = PendingIntent.GetActivity(this, pendingIntentId, intent, PendingIntentFlags.OneShot);
            var builder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context, CHANNEL_ID)
                            //.SetContentIntent(pendingIntent)
                            .SetContentTitle(title)
                            .SetDefaults((int)NotificationDefaults.All)
                            .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                            .SetContentText(content);
            //var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(this);
            var notificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(NOTIFICATION_ID, builder.Build());
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) // required only since API 26
                return;
            var channelName = "Timeinator";
            var channelDescription = "Timeinator session notifications";
            var channel = new NotificationChannel(CHANNEL_ID, channelName, NotificationImportance.High)
            {
                Description = channelDescription,
                LockscreenVisibility = NotificationVisibility.Public
            };
            var notificationManager = (NotificationManager)Application.Context.GetSystemService(Context.NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}