using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Java.Util;

namespace Timeinator.Mobile.Droid
{
    /// <summary>
    /// Service to handle session in the background
    /// </summary>
    [Service]
    public class TaskIntentService : IntentService
    {
        #region Private members

        private static TaskIntentService Instance = null;

        private static readonly int NOTIFICATION_ID = 3333;
        private static Timer TaskTimer = new Timer();
        private DateTime TaskStart { get; set; }
        private TimeSpan TaskTime { get; set; }
        private double RecentProgress { get; set; }
        private Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder { get; set; }

        private readonly IUserTimeHandler mAndroidTimeHandler;

        #endregion

        #region Constructor

        public TaskIntentService(IUserTimeHandler userTimeHandler) : base("TaskIntentService")
        {
            mAndroidTimeHandler = userTimeHandler;
        }

        public override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        #endregion

        /// <summary>
        /// Handle Task and operations
        /// </summary>
        protected override void OnHandleIntent(Intent intent)
        {
            if (intent.Action == IntentActions.ACTION_NEXTTASK)
            {
                return;
            }
            else if (intent.Action == IntentActions.ACTION_PAUSETASK)
                return;
            else if (intent.Action == IntentActions.ACTION_RESUMETASK)
                return;
        }

        #region Public methods

        /// <summary>
        /// Current Notification to notify by Service
        /// </summary>
        public Notification GetNotification()
        {
            if (NotificationBuilder == null)
            {
                NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context)
                                .SetContentIntent(GetPendingIndent(NotificationAction.GoToSession, NOTIFICATION_ID))
                                .SetSmallIcon(Resource.Mipmap.logo);
            }
            var progress = (int)(100 * (RecentProgress + (1.0 - RecentProgress) * (DateTime.Now.Subtract(TaskStart).TotalMilliseconds / TaskTime.TotalMilliseconds)));
            NotificationBuilder.SetContentTitle("Current task")
                .SetTicker("Timeinator Session")
                .SetContentText($"{progress} %")
                .SetProgress(100, progress, false);
            return NotificationBuilder.Build();
        }

        /// <summary>
        /// Start Android TaskService with Notification
        /// </summary>
        public void TaskServiceStart(TimeSpan assignedTime, double recentProgress = 0)
        {
            TaskTime = assignedTime;
            TaskStart = DateTime.Now;
            RecentProgress = recentProgress;
            var intent = new Intent(this, typeof(TaskIntentService));
            intent.PutExtra("TaskTime", TaskTime.Ticks);
            StartForegroundService(intent);
            StartForeground(NOTIFICATION_ID, GetNotification());
        }

        /// <summary>
        /// Stop Android TaskService
        /// </summary>
        public void TaskServiceStop()
        {
            StopSelf();
        }

        /// <summary>
        /// Checks if instance is created
        /// </summary>
        public bool IsRunning()
        {
            return Instance != null;
        }

        #endregion

        public static PendingIntent GetPendingIndent(NotificationAction action, int nid)
        {
            var intent = new Intent(Application.Context, typeof(ActionActivity));
            intent.SetAction(IntentActions.FromEnum(action));
            intent.PutExtra("NID", nid);
            intent.AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);
            return pendingIntent;
        }
    }
}