using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Dna;
using System;
using Timeinator.Core;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Low-level Android Service handling session in the background
    /// </summary>
    [Service]
    public class TaskService : Service
    {
        #region Private Members

        public static readonly int NOTIFICATION_ID = 3333, REFRESH_RATE = 1000;
        public static readonly string CHANNEL_ID = "com.gummybearstudio.timeinator";
        
        private ITimeTasksService mTimeTasksService = Framework.Service<ITimeTasksService>();

        #endregion

        #region Public Properties

        public NotificationCompat.Builder NotificationBuilder { get; set; }
        public NotificationManager NotificationManager { get; set; }
            = Application.Context.GetSystemService(Context.NotificationService) as NotificationManager;

        public bool IsRunning { get; set; }

        public event Action<AppAction> RequestHandler;

        public string ParamName { get; set; } = LocalizationResource.TaskName;
        public TimeSpan ParamTime { get; set; } = TimeSpan.Zero;
        public double ParamRecentProgress { get; set; } = 0;

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
                    .SetSmallIcon(Resource.Drawable.logo)
                    .SetTicker("Timeinator Session"); // TODO: Decide if this should be localized ? (Sesja for polish, session for english)
            }

            // Prepare data for notification
            var progress = mTimeTasksService.CurrentTaskCalculatedProgress * 100;
            NotificationBuilder.SetContentTitle(ParamName);

            // Remove all buttons and add new ones
            NotificationBuilder.MActions.Clear();
            // Task in progress notification
            NotificationBuilder.SetProgress(100, (int)progress, false);
            AddButton(1, LocalizationResource.Finish, IntentActions.ACTION_NEXTTASK);
            if (IsRunning)
            {
                NotificationBuilder.SetContentText(string.Format("{0:hh\\:mm\\:ss} ({1}%)", mTimeTasksService.CurrentTaskTimeLeft, progress));
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
            if (NotificationManager.GetNotificationChannel(CHANNEL_ID) != null)
                return;
            var channel = new NotificationChannel(CHANNEL_ID, "Timeinator Sessions", NotificationImportance.Default)
            {
                Description = "Background session monitoring"
            };
            NotificationManager.CreateNotificationChannel(channel);
        }

        private void AddButton(int id, string title, string action)
        {
            var intent = new Intent(Application.Context, typeof(TaskService));
            intent.SetAction(action).AddFlags(ActivityFlags.FromBackground);
            var pendingIntent = PendingIntent.GetService(this, id, intent, PendingIntentFlags.Immutable);
            NotificationBuilder.AddAction(Resource.Drawable.logo, title, pendingIntent);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Call any action supported by Service
        /// </summary>
        public void HandleMessage(Intent intent)
        {
            var appAction = default(AppAction);
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
                default:
                    {
                        StopSelf();
                        return;
                    }
            }
            RequestHandler.Invoke(appAction);
            ReNotify();
        }

        /// <summary>
        /// Refresh notification
        /// </summary>
        public void ReNotify()
        {
            NotificationManager.Notify(NOTIFICATION_ID, GetNotification());
        }

        /// <summary>
        /// Update information about current Task
        /// </summary>
        public void UpdateTask(string name, TimeSpan time, double progress)
        {
            ParamName = name;
            ParamTime = time;
            ParamRecentProgress = progress;
        }

        #endregion
    }
}
