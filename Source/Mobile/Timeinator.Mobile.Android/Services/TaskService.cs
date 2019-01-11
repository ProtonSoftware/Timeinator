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
    [Service(IsolatedProcess=true)]
    public class TaskService : Service, ITaskService
    {
        #region Private members

        private IUserTimeHandler mAndroidTimeHandler;
        private ITimeTasksService mTimeTasksService;
        private static TaskService Instance = null;

        private string TaskName { get; set; }
        private DateTime TaskStart { get; set; }
        private TimeSpan TaskTime { get; set; }
        private double RecentProgress { get; set; }
        private Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder { get; set; }

        #endregion

        #region Constructor

        public TaskService() : base()
        {
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
            StartForeground(NOTIFICATION_ID, GetNotification());
            return StartCommandResult.Sticky;
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }

        public override IBinder OnBind(Intent intent)
        {
            // Check and handle any incoming action
            HandleMessage(intent);

            Binder = new TaskServiceBinder(this);
            return Binder;
        }

        /// <summary>
        /// Perform any action supported by Service
        /// </summary>
        public void HandleMessage(Intent intent)
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
            else if (intent.Action == IntentActions.ACTION_STOP)
                StopTaskService();
        }

        /// <summary>
        /// Stops service immediately
        /// </summary>
        public void StopTaskService()
        {
            StopSelf();
        }

        /// <summary>
        /// Current Notification to notify by Service
        /// </summary>
        public Notification GetNotification()
        {
            if (NotificationBuilder == null)
            {
                var intent = new Intent(Application.Context, typeof(ActionActivity));
                intent.SetAction(IntentActions.FromEnum(AppAction.GoToSession));
                intent.PutExtra("NID", NOTIFICATION_ID);
                intent.AddFlags(ActivityFlags.ClearTop);
                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);
                NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context)
                                .SetContentIntent(pendingIntent)
                                .SetSmallIcon(Resource.Mipmap.logo);
            }
            var progress = (int)(100 * (RecentProgress + (1.0 - RecentProgress) * (DateTime.Now.Subtract(TaskStart).TotalMilliseconds / TaskTime.TotalMilliseconds)));
            NotificationBuilder.SetContentTitle(TaskName)
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

        /// <summary>
        /// Setup references to Timeinator DI
        /// </summary>
        public void RefreshService(IUserTimeHandler androidTimeHandler, ITimeTasksService timeTasksService)
        {
            mAndroidTimeHandler = androidTimeHandler;
            mTimeTasksService = timeTasksService;
        }

        #endregion
    }
}
