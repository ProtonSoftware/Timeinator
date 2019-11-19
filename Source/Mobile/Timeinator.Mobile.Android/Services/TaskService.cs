using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using System;
using Timeinator.Core;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The Android service that helps with handling session in the background while the app is minimized
    /// </summary>
    [Service]
    public class TaskService : Service
    {
        #region Private Members

        // Configuration
        // TODO: Move this somewhere else, but for now it works just fine with being here
        private static readonly int NotificationId = 3333;
        private static readonly string ChannelId = "com.gummybearstudio.timeinator";

        /// <summary>
        /// The notification manager for every interactions with actual notification building process
        /// </summary>
        private readonly AndroidNotificationManager mNotificationManager = new AndroidNotificationManager(ChannelId);

        #endregion

        #region Public Properties

        /// <summary>
        /// TODO: Try to remove this binder reference here and see if app still works fine in background
        /// </summary>
        public IBinder Binder { get; private set; }

        /// <summary>
        /// The current task's name to show as a title of notification
        /// </summary>
        public string Title { get; set; } = LocalizationResource.TaskName;

        /// <summary>
        /// The time left for the current task to be finished 
        /// </summary>
        public TimeSpan Time { get; set; }

        /// <summary>
        /// The percentage progress of current task
        /// </summary>
        public int Progress { get; set; }

        /// <summary>
        /// Indicates if notification is currently up and running
        /// </summary>
        public bool IsRunning { get; set; }

        /// <summary>
        /// The event to handle any request coming from notification
        /// </summary>
        public event Action<AppAction> RequestHandler;

        #endregion

        #region Public Methods

        /// <summary>
        /// Called initially when this service is being created
        /// </summary>
        public override void OnCreate()
        {
            // Do base stuff
            base.OnCreate();

            // Create notification channel
            mNotificationManager.CreateNotificationChannel(ChannelId);

            // Create notification itself and start displaying it
            StartForeground(NotificationId, GetNotification());
        }

        /// <summary>
        /// TODO: Explaination what this function really does
        /// </summary>
        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            // Use provided intent to do requested action
            HandleMessage(intent);

            // TODO: Explanation why its NotSticky here plz
            return StartCommandResult.NotSticky;
        }

        /// <summary>
        /// Called when new binder is requested for this service
        /// </summary>
        /// <param name="intent">The action to perform while creating new binder</param>
        /// <returns>Created Android Binder</returns>
        public override IBinder OnBind(Intent intent)
        {
            // Use provided intent to do requested action
            HandleMessage(intent);

            // Reset the event handler
            RequestHandler = (a) => { };

            // Create new binder with this service inside
            Binder = new TaskServiceBinder(this);

            // Return the binder itself
            return Binder;
        }

        /// <summary>
        /// Refreshes current notification state with newest available data
        /// </summary>
        public void ReNotify() => mNotificationManager.Notify(NotificationId, GetNotification());

        #endregion

        #region Private Helpers

        /// <summary>
        /// Creates brand-new notification using the manager
        /// </summary>
        public Notification GetNotification() => mNotificationManager.CreateNotification(Title, Progress, Time, this);

        /// <summary>
        /// Runs any action from provided intent
        /// </summary>
        public void HandleMessage(Intent intent)
        {
            // Get the user action based on provided intent
            var appAction = intent.Action.ToActionEnum();

            // If the action was to stop the session...
            if (appAction == AppAction.StopSession)
            {
                // Kill the notification
                StopForeground(true);
                StopSelf();
                return;
            }

            // Perform desired action
            RequestHandler.Invoke(appAction);

            // Refresh the notification
            ReNotify();
        }

        #endregion
    }
}
