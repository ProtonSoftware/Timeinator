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
    /// Low-level Android Service handling session in the background
    /// </summary>
    [Service(IsolatedProcess=true)]
    public class TaskService : Service, ITaskService
    {
        #region Private members

        private IUserTimeHandler mAndroidTimeHandler;
        private static TaskService Instance = null;

        private Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder { get; set; }
        private Timer TaskTimer { get; set; }

        private string TaskName { get; set; }
        private DateTime TaskStart { get; set; }
        private TimeSpan TaskTime { get; set; }
        private double RecentProgress { get; set; }

        #endregion

        #region Constructor

        public TaskService() : base()
        {
        }

        #endregion

        #region Public methods

        public static readonly int NOTIFICATION_ID = 3333, REFRESH_RATE = 5;
        public IBinder Binder { get; private set; }

        public override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            TaskTimer = new Timer();
            TaskTimer.ScheduleAtFixedRate(new ServiceRefresh(this), TaskTime.Ticks, TimeSpan.FromSeconds(REFRESH_RATE).Ticks);
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
                mAndroidTimeHandler.RemoveAndContinueTasks();
            }
            else if (intent.Action == IntentActions.ACTION_PAUSETASK)
            {
                mAndroidTimeHandler.StopTask();
            }
            else if (intent.Action == IntentActions.ACTION_RESUMETASK)
            {
                mAndroidTimeHandler.ResumeTask();
            }
            if (intent.Action == IntentActions.ACTION_STOP)
                StopTaskService();
            else
                CollectTaskInfo();
        }

        /// <summary>
        /// Stops service immediately
        /// </summary>
        public void StopTaskService()
        {
            TaskTimer.Dispose();
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
        public void RefreshService(IUserTimeHandler androidTimeHandler) // REMOVE THIS BAD CODE
        {
            mAndroidTimeHandler = androidTimeHandler;
            CollectTaskInfo();
        }

        /// <summary>
        /// Execute when task time is over
        /// </summary>
        public void EndOfTask()
        {
            mAndroidTimeHandler.InvokeTimesUp();
            var intent = new Intent();
            intent.SetAction(IntentActions.ACTION_NEXTTASK);
            HandleMessage(intent);
        }

        public TimeSpan TimeRemaining() => TaskStart.Add(TaskTime) - DateTime.Now;

        #endregion

        #region Private Methods

        /// <summary>
        /// Loads data about CurrentTask to local properties
        /// </summary>
        private void CollectTaskInfo() // REMOVE THIS BAD CODE!!! - PROVIDE LOW-API INSTEAD AND USE TaskServiceConnection TO HANDLE REQUESTS
        {
            var task = mAndroidTimeHandler.CurrentTask;
            if (task == null)
                return;
            TaskName = task.Name;
            TaskStart = mAndroidTimeHandler.CurrentTaskStartTime;
            TaskTime = task.AssignedTime;
            RecentProgress = mAndroidTimeHandler.RecentProgress;
        }

        /// <summary>
        /// Run fired every REFRESH_RATE period, updates Service state
        /// </summary>
        private class ServiceRefresh : TimerTask
        {
            public ServiceRefresh(TaskService service) : base()
            {
                mService = service;
            }

            private TaskService mService;

            public override void Run()
            {
                NotificationHandler.NManager.Notify(NOTIFICATION_ID, mService.GetNotification());
                // Check if time has passed
                if (mService.TimeRemaining().Ticks < 0)
                    mService.EndOfTask();
            }
        }

        #endregion
    }
}
