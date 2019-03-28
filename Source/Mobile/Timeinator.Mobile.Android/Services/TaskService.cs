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
        public NotificationManager NManager => Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        #region Private members

        private static TaskService Instance = null;

        private Android.Support.V4.App.NotificationCompat.Builder NotificationBuilder { get; set; }
        private Timer TaskTimer { get; set; }

        #endregion

        #region Constructor

        public TaskService() : base()
        {
        }

        #endregion

        #region Public methods
        //public string Name { get; set; }
        //public DateTime Start { get; set; }
        //public TimeSpan Time { get; set; }
        //public double RecentProgress { get; set; }

        //public event Action TimerElapsed;

        public static readonly int NOTIFICATION_ID = 3333, REFRESH_RATE = 5;
        public IBinder Binder { get; private set; }

        //public TimeSpan TimeRemaining() => Start.Add(Time) - DateTime.Now;

        public override void OnCreate()
        {
            base.OnCreate();
            Instance = this;
            //TimerElapsed = () => { };
            //CreateTimer();
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
            //if (intent.Action == IntentActions.ACTION_NEXTTASK)
            //    TimerElapsed.Invoke();
            //else if (intent.Action == IntentActions.ACTION_PAUSETASK)
            //    TaskTimer.Cancel();
            //else if (intent.Action == IntentActions.ACTION_RESUMETASK)
            //    CreateTimer();
            //if (intent.Action == IntentActions.ACTION_STOP)
            //    StopTaskService();
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
                var intent = new Intent(Application.Context, typeof(MainActivity));
                intent.SetAction(IntentActions.FromEnum(AppAction.GoToSession));
                intent.PutExtra("NID", NOTIFICATION_ID);
                intent.AddFlags(ActivityFlags.ClearTop);
                var pendingIntent = PendingIntent.GetActivity(Application.Context, 0, intent, PendingIntentFlags.Immutable);
                NotificationBuilder = new Android.Support.V4.App.NotificationCompat.Builder(Application.Context)
                                .SetContentIntent(pendingIntent)
                                .SetSmallIcon(Resource.Mipmap.logo);
            }
            //var progress = (int)(100 * (RecentProgress + (1.0 - RecentProgress) * (DateTime.Now.Subtract(Start).TotalMilliseconds / Time.TotalMilliseconds)));
            //NotificationBuilder.SetContentTitle(Name)
            var progress = (int)(100 * 0.5);
            NotificationBuilder.SetContentTitle("TEST")
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
        /// Execute when task time is over
        /// </summary>
        public void EndOfTime()
        {
            var intent = new Intent();
            intent.SetAction(IntentActions.ACTION_NEXTTASK);
            HandleMessage(intent);
        }

        #endregion

        #region Private Methods

        private void CreateTimer()
        {
            if (TaskTimer != null)
                TaskTimer.Dispose();
            TaskTimer = new Timer();
            //TaskTimer.ScheduleAtFixedRate(new ServiceRefresh(this), Time.Ticks, TimeSpan.FromSeconds(REFRESH_RATE).Ticks);
        }

        #endregion
    }
}
