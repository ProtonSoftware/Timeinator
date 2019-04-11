using System;
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
        public static readonly int NOTIFICATION_ID = 3333, REFRESH_RATE = 1500;
        public static readonly string CHANNEL_ID = "com.gummybearstudio.timeinator";

        #region Private members

        private class Timer : CountDownTimer
        {
            private TaskService mParent;

            public Timer(long millis, long interval, TaskService parent) : base(millis, interval)
            {
                mParent = parent;
            }

            public override void OnTick(long millisUntilFinished) => mParent.ReNotify();

            public override void OnFinish() => mParent.TimeOut();
        }

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
            HandleMessage(intent);
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
            RequestHandler = (a) => { };
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
            // Prepare Intent opening App
            var intent = new Intent(Application.Context, typeof(MainActivity));
            intent.SetAction(IntentActions.ACTION_GOSESSION).AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);

            // Create NotificationBuilder if not existing yet
            if (NotificationBuilder == null)
            {
                NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context, CHANNEL_ID)
                    .SetOngoing(true)
                    .SetContentIntent(pendingIntent)
                    .SetSmallIcon(Resource.Mipmap.logo)
                    .SetTicker("Timeinator Session");
            }

            // Set information on Notification
            var timePassed = DateTime.Now.Subtract(ParamStart);
            var progress = 0;
            if (ParamTime.TotalMilliseconds > 0)
                progress = (int)(100 * (ParamRecentProgress + (1.0 - ParamRecentProgress) * (timePassed.TotalMilliseconds / ParamTime.TotalMilliseconds)));
            NotificationBuilder.SetContentTitle(ParamName);

            // Remove all buttons and add new ones
            NotificationBuilder.MActions.Clear();
            // Task finished notification
            if (progress >= 100)
            {
                NotificationBuilder.SetContentText("Task finished").SetProgress(0, 0, false)
                    .AddAction(Resource.Mipmap.icon_round, "Next", pendingIntent);
            }
            // Task in progress notification
            else
            {
                NotificationBuilder.SetProgress(100, progress, false);
                AddButton(1, "Finish", IntentActions.ACTION_NEXTTASK);
                if (Running)
                {
                    NotificationBuilder.SetContentText(string.Format("{0:hh\\:mm\\:ss} ({1}%)", ParamTime - timePassed, progress));
                    AddButton(2, "Pause", IntentActions.ACTION_PAUSETASK);
                }
                else
                {
                    NotificationBuilder.SetContentText(string.Format("Session paused ({0}%)", progress));
                    AddButton(3, "Resume", IntentActions.ACTION_RESUMETASK);
                }
            }

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

        private void AddButton(int id, string title, string action)
        {
            var intent = new Intent(Application.Context, typeof(TaskService));
            intent.SetAction(action).AddFlags(ActivityFlags.FromBackground);
            var pendingIntent = PendingIntent.GetService(this, id, intent, PendingIntentFlags.Immutable);
            NotificationBuilder.AddAction(Resource.Mipmap.icon_round, title, pendingIntent);
        }

        #endregion

        public bool Running => TaskTiming != null;

        public event Action Elapsed;
        public event Action<AppAction> RequestHandler;

        public string ParamName { get; set; } = "None";
        public DateTime ParamStart { get; set; } = DateTime.Now;
        public TimeSpan ParamTime { get; set; } = TimeSpan.Zero;
        public double ParamRecentProgress { get; set; } = 0;

        /// <summary>
        /// Call any action supported by Service
        /// </summary>
        public void HandleMessage(Intent intent)
        {
            if (intent.Action == IntentActions.ACTION_STOP)
                KillService();
            else
            {
                if (intent.Action == IntentActions.ACTION_NEXTTASK)
                    RequestHandler.Invoke(AppAction.NextSessionTask);
                else if (intent.Action == IntentActions.ACTION_RESUMETASK)
                    RequestHandler.Invoke(AppAction.ResumeSession);
                else if (intent.Action == IntentActions.ACTION_PAUSETASK)
                    RequestHandler.Invoke(AppAction.PauseSession);
                ReNotify();
            }
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
            if (Running)
            {
                TaskTiming.Cancel();
                TaskTiming.Dispose();
                TaskTiming = null;
            }
        }

        /// <summary>
        /// Start background timing with notification refreshing
        /// </summary>
        public void Start()
        {
            if (ParamTime.Ticks <= 0)
                return;
            Stop();
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
        public void UpdateTask(string n, DateTime s, TimeSpan t, double p)
        {
            ParamName = n;
            UpdateTask(s, t, p);
        }

        /// <summary>
        /// Update information about current Task
        /// </summary>
        public void UpdateTask(DateTime s, TimeSpan t, double p)
        {
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
            ReNotify();
        }

        /// <summary>
        /// Stops service immediately
        /// </summary>
        public void KillService() => StopSelf();
    }
}
