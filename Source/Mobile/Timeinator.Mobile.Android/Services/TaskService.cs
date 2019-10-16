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
    /// Low-level Android Service handling session in the background
    /// </summary>
    [Service]
    public class TaskService : Service
    {
        #region Private Members

        private static readonly int NotificationId = 3333;
        private static readonly string ChannelId = "com.gummybearstudio.timeinator";

        private readonly AndroidNotificationManager mNotificationManager = new AndroidNotificationManager(ChannelId);

        #endregion

        #region Public Properties

        public string Title { get; set; } = LocalizationResource.TaskName;
        public TimeSpan Time { get; set; } = default;
        public double Progress { get; set; }

        public bool IsRunning { get; set; }
        public event Action<AppAction> RequestHandler;

        #endregion

        #region Public Service

        public IBinder Binder { get; private set; }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            mNotificationManager.CreateNotificationChannel(ChannelId);
            StartForeground(NotificationId, GetNotification());
            HandleMessage(intent);
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            // Check and handle any incoming action
            HandleMessage(intent);
            RequestHandler = (a) => { };
            Binder = new TaskServiceBinder(this);
            return Binder;
        }

        public new void StopSelf()
        {
            // Kill the notification
            StopForeground(true);

            // Do base service stopping stuff
            base.StopSelf();
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Current Notification to notify by Service
        /// </summary>
        public Notification GetNotification()
        {
            // Prepare data for notification
            var progress = Progress * 100;

            // Create notification and return it
            return mNotificationManager.CreateNotification(Title, (int)progress, Time, this);
        }

        /// <summary>
        /// Call any action supported by Service
        /// </summary>
        public void HandleMessage(Intent intent)
        {
            AppAction appAction;
            switch (intent.Action)
            {
                case IntentActions.ACTION_NEXTTASK:
                    appAction = AppAction.NextSessionTask;
                    break;

                case IntentActions.ACTION_RESUMETASK:
                    appAction = AppAction.ResumeSession;
                    break;

                case IntentActions.ACTION_PAUSETASK:
                    appAction = AppAction.PauseSession;
                    break;

                case IntentActions.ACTION_STOP:
                    {
                        StopSelf();
                        return;
                    }
                    
                default:
                    return;
            }
            RequestHandler.Invoke(appAction);
            ReNotify();
        }

        /// <summary>
        /// Refresh notification
        /// </summary>
        public void ReNotify()
        {
            mNotificationManager.Notify(NotificationId, GetNotification());
        }

        #endregion
    }
}
