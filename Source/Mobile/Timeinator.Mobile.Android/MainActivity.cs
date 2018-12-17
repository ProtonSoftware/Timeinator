using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using Android.Content;

namespace Timeinator.Mobile.Droid
{
    [Activity(Label = "Timeinator", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            CreateNotificationChannel();
            base.OnCreate(savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
        }

        /// <summary>
        /// Builds basic notification redirecting to session page and shows it
        /// </summary>
        public void BuildNotification(string title, string content)
        {
            var resultIntent = new Intent(this, typeof(RedirectActivity));
            // Construct a back stack for cross-task navigation:
            var stackBuilder = Android.Support.V4.App.TaskStackBuilder.Create(this);
            stackBuilder.AddParentStack(new RedirectActivity());
            stackBuilder.AddNextIntent(resultIntent);
            // Create the PendingIntent with the back stack:
            var resultPendingIntent = stackBuilder.GetPendingIntent(0, (int)PendingIntentFlags.UpdateCurrent);
            var builder = new Android.Support.V4.App.NotificationCompat.Builder(this, CHANNEL_ID)
                            .SetContentIntent(resultPendingIntent)
                            .SetContentTitle(title)
                            .SetDefaults((int)NotificationDefaults.All)
                            .SetSmallIcon(Resource.Drawable.notification_template_icon_bg)
                            .SetContentText(content);
            //var notificationManager = Android.Support.V4.App.NotificationManagerCompat.From(this);
            var notificationManager = GetSystemService(Context.NotificationService) as NotificationManager;
            notificationManager.Notify(NOTIFICATION_ID, builder.Build());
        }

        private static readonly int NOTIFICATION_ID = 3030;
        private static readonly string CHANNEL_ID = "session_notification";

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
            var notificationManager = (NotificationManager)GetSystemService(NotificationService);
            notificationManager.CreateNotificationChannel(channel);
        }
    }
}