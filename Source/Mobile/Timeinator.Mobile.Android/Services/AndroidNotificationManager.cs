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
        private readonly NotificationCompat.Builder mNotificationBuilder;

        /// <summary>
        /// Native android notification manager
        /// </summary>
        private readonly NotificationManager mNotificationManager;

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor that takes in notification channel ID
        /// </summary>
        /// <param name="channelId">The id for notification channel</param>
        public AndroidNotificationManager(string channelId)
        {
            // Get default android notification manager
            mNotificationManager = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

            // Prepare intent for opening app by notification click
            var intent = new Intent(Application.Context, typeof(LoginPage));
            intent.SetAction(IntentActions.ACTION_GOSESSION).AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);

            // Create notification builder providing notification data
            mNotificationBuilder = new NotificationCompat.Builder(Application.Context, channelId)
                .SetOnlyAlertOnce(true)
                .SetOngoing(true)
                .SetContentIntent(pendingIntent)
                .SetSmallIcon(Resource.Drawable.logo)
                .SetTicker(LocalizationResource.TimeinatorSessionTitle);
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates notification channel based on provided id
        /// </summary>
        /// <param name="channelId">The id for notification channel</param>
        public void CreateNotificationChannel(string channelId)
        {
            // This feature is only for Android 8.0+
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;

            // Don't create new channel if there is one already
            if (mNotificationManager.GetNotificationChannel(channelId) != null)
                return;

            // Create new channel
            var channel = new NotificationChannel(channelId, "Timeinator Sessions", NotificationImportance.Default)
            {
                Description = "Background session monitoring"
            };

            // Pass it to the default notification manager
            mNotificationManager.CreateNotificationChannel(channel);
        }

        /// <summary>
        /// Creates Timeinator session notification
        /// </summary>
        /// <param name="title">The title of notification, usually the current task's name</param>
        /// <param name="progress">The progress of current task</param>
        /// <param name="time">The time left of current task</param>
        /// <param name="taskService">The instance of task service which is running in the background</param>
        /// <returns></returns>
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

            // Finally build the notification and return it
            return mNotificationBuilder.Build();
        }

        /// <summary>
        /// Passes the notify alert to update data to the default notification manager
        /// </summary>
        /// <param name="notificationId">The notification ID</param>
        /// <param name="notification">The actual notification instance</param>
        public void Notify(int notificationId, Notification notification) => mNotificationManager.Notify(notificationId, notification);

        /// <summary>
        /// Adds clickable button onto the notification
        /// </summary>
        /// <param name="id">The ID of button to add, used for identification purposes as well as ordering</param>
        /// <param name="title">The title to display on button</param>
        /// <param name="action">The name of the action to perform when button is clicked</param>
        /// <param name="taskService">The instance of task service which is running in the background</param>
        public void AddButton(int id, string title, string action, TaskService taskService)
        {
            // Prepare new intent for task service instance
            var intent = new Intent(Application.Context, typeof(TaskService));
            intent.SetAction(action).AddFlags(ActivityFlags.FromBackground);
            var pendingIntent = PendingIntent.GetService(taskService, id, intent, PendingIntentFlags.UpdateCurrent);

            // Add button action to the notification
            mNotificationBuilder.AddAction(Resource.Drawable.logo, title, pendingIntent);
        }

        /// <summary>
        /// Kills all notifications
        /// </summary>
        public void Kill()
        {
            // Cancel out all shown notifications
            mNotificationManager.CancelAll();
        }

        #endregion
    }
}