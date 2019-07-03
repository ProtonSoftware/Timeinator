using Android.App;
using Android.Content;
using Android.OS;
using Android.Support.V4.App;
using System;
using Timeinator.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The manager for android session notification
    /// </summary>
    public class AndroidNotificationManager
    {
        #region Private Members

        /// <summary>
        /// Native android notification builder
        /// </summary>
        private NotificationCompat.Builder mNotificationBuilder;

        /// <summary>
        /// Native android notification manager
        /// </summary>
        private NotificationManager mNotificationManager;

        #endregion

        #region Public Methods

        public AndroidNotificationManager(string channelId)
        {
            mNotificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            // Prepare Intent opening App
            var intent = new Intent(Application.Context, typeof(TasksSessionPage));
            intent.SetAction(IntentActions.ACTION_GOSESSION).AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);

            mNotificationBuilder = new NotificationCompat.Builder(Application.Context, channelId)
                .SetOnlyAlertOnce(true)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent)
                .SetSmallIcon(Resource.Drawable.logo)
                .SetTicker("Timeinator Session"); // TODO: Decide if this should be localized ? (Sesja for polish, session for english)
        }

        public void CreateNotificationChannel(string channelId)
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;
            if (mNotificationManager.GetNotificationChannel(channelId) != null)
                return;
            var channel = new NotificationChannel(channelId, "Timeinator Sessions", NotificationImportance.Default)
            {
                Description = "Background session monitoring"
            };
            mNotificationManager.CreateNotificationChannel(channel);
        }

        public Notification CreateNotification(string title, int progress, TimeSpan time, TaskService taskService)
        {
            // Set provided title in the notification 
            mNotificationBuilder.SetContentTitle(title);

            // Set progress bar
            mNotificationBuilder.SetProgress(100, progress, false);

            // Remove all previous buttons
            mNotificationBuilder.MActions.Clear();

            // Add the button for finishing task
            AddButton(1, LocalizationResource.Finish, IntentActions.ACTION_NEXTTASK, taskService);

            // If task is currently running...
            if (taskService.IsRunning)
            {
                // Set appropriate text with progress
                mNotificationBuilder.SetContentText(string.Format("{0:hh\\:mm\\:ss} ({1}%)", time, progress));

                // Add pause button
                AddButton(2, LocalizationResource.Pause, IntentActions.ACTION_PAUSETASK, taskService);
            }
            // Otherwise...
            else
            {
                // Set appropriate text
                mNotificationBuilder.SetContentText(string.Format(LocalizationResource.SessionPausedWithProgress, progress));

                // Add resume button
                AddButton(3, LocalizationResource.Resume, IntentActions.ACTION_RESUMETASK, taskService);
            }

            return mNotificationBuilder.Build();
        }

        public void Notify(int notificationId, Notification notification)
        {
            mNotificationManager.Notify(notificationId, notification);
        }

        public void AddButton(int id, string title, string action, TaskService taskService)
        {
            var intent = new Intent(Application.Context, typeof(TaskService));
            intent.SetAction(action).AddFlags(ActivityFlags.FromBackground);
            var pendingIntent = PendingIntent.GetService(taskService, id, intent, PendingIntentFlags.Immutable);
            mNotificationBuilder.AddAction(Resource.Drawable.logo, title, pendingIntent);
        }

        #endregion
    }
}