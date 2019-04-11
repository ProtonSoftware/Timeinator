﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

namespace Timeinator.Mobile.Droid
{
    // [Service(IsolatedProcess=true)]
    /// <summary>
    /// Low-level Android Service handling session in the background
    /// </summary>
    [Service]
    public class TaskService : Service
    {
        private class Timer : CountDownTimer
        {
            private TaskService mParent;

            public Timer(long millis, long interval, TaskService parent) : base(millis, interval)
            {
                mParent = parent;
            }

            public override void OnTick(long millisUntilFinished)
            {
            }

            public override void OnFinish() => mParent.TimeOut();
        }

        public static readonly int NOTIFICATION_ID = 3333, REFRESH_RATE = 5000;
        public static readonly string CHANNEL_ID = "com.gummybearstudio.timeinator";

        #region Private members

        private Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder { get; set; }
        private NotificationManager NManager => Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        private Timer TaskTiming { get; set; } = null;

        #endregion

        #region Constructor

        public TaskService() : base()
        {
        }

        #endregion

        #region Public methods

        public IBinder Binder { get; private set; }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            CreateNotificationChannel();
            StartForeground(NOTIFICATION_ID, GetNotification());
            return StartCommandResult.Sticky;
        }

        public override void OnCreate()
        {
            base.OnCreate();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        public override IBinder OnBind(Intent intent)
        {
            // Check and handle any incoming action
            HandleMessage(intent);
            Elapsed = () => { };
            Binder = new TaskServiceBinder(this);
            return Binder;
        }

        #endregion

        #region Notifications

        /// <summary>
        /// Current Notification to notify by Service
        /// </summary>
        public Notification GetNotification()
        {
            if (NotificationBuilder == null)
            {
                var intent = new Intent(Application.Context, typeof(MainActivity));
                intent.SetAction(IntentActions.FromEnum(AppAction.GoToSession)).PutExtra("NID", NOTIFICATION_ID).AddFlags(ActivityFlags.ClearTop);
                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);
                NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context, CHANNEL_ID)
                    .SetOngoing(true)
                    .SetContentIntent(pendingIntent)
                    .SetSmallIcon(Resource.Mipmap.logo);
            }
            var timeLeft = DateTime.Now.Subtract(ParamStart);
            var progress = 0;
            if (ParamTime.TotalMilliseconds > 0)
                progress = (int)(100 * (ParamRecentProgress + (1.0 - ParamRecentProgress) * (timeLeft.TotalMilliseconds / ParamTime.TotalMilliseconds)));
            NotificationBuilder.SetContentTitle(ParamName)
                .SetTicker("Timeinator Session");
            if (progress > 100)
                NotificationBuilder.SetContentText("Task finished").SetProgress(0, 0, false);
            else
                NotificationBuilder.SetContentText($"{timeLeft} ({progress}%)").SetProgress(100, progress, false);
            return NotificationBuilder.Build();
        }

        private void CreateNotificationChannel()
        {
            if (Build.VERSION.SdkInt < BuildVersionCodes.O)
                return;
            if (NManager.GetNotificationChannel(CHANNEL_ID) != null)
                return;
            var channel = new NotificationChannel(CHANNEL_ID, "Timeinator Sessions", NotificationImportance.Default)
            {
                Description = "Background session monitoring"
            };
            NManager.CreateNotificationChannel(channel);
        }

        #endregion

        public string ParamName { get; set; } = "None";
        public DateTime ParamStart { get; set; } = DateTime.Now;
        public TimeSpan ParamTime { get; set; } = TimeSpan.Zero;
        public double ParamRecentProgress { get; set; } = 0;
        public event Action Elapsed;

        /// <summary>
        /// Call any action supported by Service
        /// </summary>
        public void HandleMessage(Intent intent)
        {
        }

        /// <summary>
        /// Refresh notification
        /// </summary>
        public void ReNotify() => NManager.Notify(NOTIFICATION_ID, GetNotification());

        /// <summary>
        /// Stops Timer countdown
        /// </summary>
        public void Stop()
        {
            if (TaskTiming != null)
                TaskTiming.Cancel();
        }

        /// <summary>
        /// Set an alarm to fire at Task time out
        /// </summary>
        public void Start()
        {
            if (ParamTime.Ticks <= 0)
                return;
            if (TaskTiming != null)
            {
                TaskTiming.Cancel();
                TaskTiming.Dispose();
            }
            TaskTiming = new Timer((long)ParamTime.TotalMilliseconds, REFRESH_RATE, this);
            TaskTiming.Start();
            // AlarmManager version below (not tested)
            /*var am = AlarmManager.FromContext(Application.Context);
            var intent = new Intent(Application.Context, typeof(TaskService));
            intent.SetAction(IntentActions.ACTION_NEXTTASK).AddFlags(ActivityFlags.FromBackground);
            var pintent = PendingIntent.GetService(Application.Context, 0, intent, PendingIntentFlags.Immutable);
            am.Set(AlarmType.ElapsedRealtimeWakeup, (long)ParamTime.TotalMilliseconds, pintent);*/
        }

        /// <summary>
        /// Update information about current Task
        /// </summary>
        public void UpdateInfo(string n, DateTime s, TimeSpan t, double p)
        {
            ParamName = n;
            ParamStart = s;
            ParamTime = t;
            ParamRecentProgress = p;
        }

        /// <summary>
        /// Execute when task time is over
        /// </summary>
        public void TimeOut()
        {
            Elapsed.Invoke();
        }

        /// <summary>
        /// Stops service immediately
        /// </summary>
        public void KillService() => StopSelf();
    }
}
