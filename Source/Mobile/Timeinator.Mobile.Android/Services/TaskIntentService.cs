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
    /// Service handling session in the background
    /// </summary>
    [Service]
    public class TaskIntentService : Service
    {
        #region Private members

        private readonly IUserTimeHandler mAndroidTimeHandler;
        private readonly ITimeTasksService mTimeTasksService;
        private static TaskIntentService Instance = null;

        private static Timer TaskTimer = new Timer();
        private DateTime TaskStart { get; set; }
        private TimeSpan TaskTime { get; set; }
        private double RecentProgress { get; set; }
        private Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder { get; set; }

        #endregion

        #region Constructor

        public TaskIntentService()
        {
        }

        public TaskIntentService(IUserTimeHandler androidTimeHandler)
        {
            mAndroidTimeHandler = androidTimeHandler;
        }

        #endregion

        #region Public methods

        public static readonly int NOTIFICATION_ID = 3333;
        public IBinder Binder { get; private set; }

        public override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
        }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        public override IBinder OnBind(Intent intent)
        {
            if (intent.Action == IntentActions.ACTION_NEXTTASK)
            {
                mAndroidTimeHandler.FinishTask();
                mAndroidTimeHandler.RemoveAndContinueTasks(mTimeTasksService);
            }
            else if (intent.Action == IntentActions.ACTION_PAUSETASK)
            {
                mAndroidTimeHandler.StopTask();
            }
            else if (intent.Action == IntentActions.ACTION_RESUMETASK)
            {
                mAndroidTimeHandler.ResumeTask();
            }
            Binder = new TaskServiceBinder(this);
            return Binder;
        }

        /// <summary>
        /// Current Notification to notify by Service
        /// </summary>
        public Notification GetNotification()
        {
            if (NotificationBuilder == null)
            {
                var intent = new Intent(Application.Context, typeof(ActionActivity));
                intent.SetAction(IntentActions.FromEnum(NotificationAction.GoToSession));
                intent.PutExtra("NID", NOTIFICATION_ID);
                intent.AddFlags(ActivityFlags.ClearTop);
                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);
                NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context)
                                .SetContentIntent(pendingIntent)
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
        /// Checks if instance is created
        /// </summary>
        public bool IsRunning()
        {
            return Instance != null;
        }

        #endregion
    }
}
