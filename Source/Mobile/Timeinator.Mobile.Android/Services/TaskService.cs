using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using System;
using Timeinator.Core;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    // [Service(IsolatedProcess=true)]
    /// <summary>
    /// Low-level Android Service handling session in the background
    /// </summary>
    [Service]
    public class TaskService : Service
    {
        public static readonly int NOTIFICATION_ID = 3333, REFRESH_RATE = 1000;
        public static readonly string CHANNEL_ID = "com.gummybearstudio.timeinator";

        #region Private members

        /* private class NotificationTimer : CountDownTimer
        {
            private TaskService mParent;

            public NotificationTimer(long millis, long interval, TaskService parent) : base(millis, interval)
            {
                mParent = parent;
            }

            public override void OnTick(long millisUntilFinished) => mParent.ReNotify();

            public override void OnFinish() => mParent.TimeOut();
        } */

        private NotificationCompat.Builder NotificationBuilder { get; set; }
        private NotificationManager NManager => Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        // private NotificationTimer TaskTimer { get; set; }

        #endregion

        #region Public Service

        public IBinder Binder { get; private set; }

        [return: GeneratedEnum]
        public override StartCommandResult OnStartCommand(Intent intent, [GeneratedEnum] StartCommandFlags flags, int startId)
        {
            CreateNotificationChannel();
            StartForeground(NOTIFICATION_ID, GetNotification());
            HandleMessage(intent);
            return StartCommandResult.Sticky;
        }

        public override IBinder OnBind(Intent intent)
        {
            // Check and handle any incoming action
            HandleMessage(intent);
            /*Elapsed = () => {
                var alarmintent = new Intent(Application.Context, typeof(AlarmPage));
                alarmintent.SetAction(IntentActions.ACTION_SHOW).AddFlags(ActivityFlags.FromBackground);
                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, alarmintent, PendingIntentFlags.Immutable);
            };*/
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
            var intent = new Intent(Application.Context, typeof(TasksSessionPage));
            intent.SetAction(IntentActions.ACTION_GOSESSION).AddFlags(ActivityFlags.ClearTop);
            var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);

            // Create NotificationBuilder if not existing yet
            if (NotificationBuilder == null)
            {
                NotificationBuilder = new NotificationCompat.Builder(Application.Context, CHANNEL_ID)
                    .SetOnlyAlertOnce(true)
                    .SetOngoing(true)
                    .SetContentIntent(pendingIntent)
                    .SetSmallIcon(Resource.Mipmap.logo)
                    .SetTicker("Timeinator Session"); // TODO: Decide if this should be localized ? (Sesja for polish, session for english)
            }

            // Set information on Notification
            var timePassed = DateTime.Now.Subtract(ParamStart);
            var progress = DI.TimeTaskService.CurrentTaskCalculatedProgress * 100;
            NotificationBuilder.SetContentTitle(ParamName);

            // Remove all buttons and add new ones
            NotificationBuilder.MActions.Clear();
            // Task in progress notification
            NotificationBuilder.SetProgress(100, (int)progress, false);
            AddButton(1, LocalizationResource.Finish, IntentActions.ACTION_NEXTTASK);
            if (Running)
            {
                NotificationBuilder.SetContentText(string.Format("{0:hh\\:mm\\:ss} ({1}%)", ParamTime - timePassed, progress));
                AddButton(2, LocalizationResource.Pause, IntentActions.ACTION_PAUSETASK);
            }
            else
            {
                NotificationBuilder.SetContentText(string.Format(LocalizationResource.SessionPausedWithProgress, progress));
                AddButton(3, LocalizationResource.Resume, IntentActions.ACTION_RESUMETASK);
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
            NotificationBuilder.AddAction(Resource.Mipmap.ic_launcher_round, title, pendingIntent);
        }

        #endregion

        #region Public Properties

        public bool Running { get; set; }

        // public event Action Elapsed = () => { }, Tick = () => { };
        public event Action<AppAction> RequestHandler;

        public string ParamName { get; set; } = "None";
        public DateTime ParamStart { get; set; } = DateTime.Now;
        public TimeSpan ParamTime { get; set; } = TimeSpan.Zero;
        public double ParamRecentProgress { get; set; } = 0;

        #endregion

        #region Public methods

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
        public void ReNotify()
        {
            // Tick.Invoke();
            NManager.Notify(NOTIFICATION_ID, GetNotification());
        }

        /// <summary>
        /// Stops Timer countdown
        /// </summary>
        public void Stop()
        {
            Running = false;
            /*
            if (Running)
            {
                TaskTimer.Cancel();
                TaskTimer.Dispose();
                TaskTimer = null;
                ReNotify();
            }*/
        }

        /// <summary>
        /// Start background timing with notification refreshing
        /// </summary>
        public void Start()
        {
            Running = true;
            /*if (ParamTime.Ticks <= 0)
                return;
            Stop();
            TaskTimer = new NotificationTimer((long)ParamTime.TotalMilliseconds, REFRESH_RATE, this);
            TaskTimer.Start();*/
        }

        /*
        /// <summary>
        /// Execute when task time is over
        /// </summary>
        public void TimeOut()
        {
            Elapsed.Invoke();
            // Open alarm page with intent
            //var intent = new Intent(Application.Context, typeof(AlarmPage));
            //intent.SetAction(IntentActions.ACTION_TIMEOUT).AddFlags(ActivityFlags.NewTask | ActivityFlags.ClearTop);
            //StartActivity(intent);
            ReNotify();
        }
        */

        /// <summary>
        /// Update information about current Task
        /// </summary>
        public void UpdateTask(string n, DateTime s, TimeSpan t, double p)
        {
            ParamName = n;
            ParamStart = s;
            ParamTime = t;
            ParamRecentProgress = p;
        }

        /// <summary>
        /// Stops service immediately
        /// </summary>
        public void KillService() => StopSelf();

        #endregion
    }
}
