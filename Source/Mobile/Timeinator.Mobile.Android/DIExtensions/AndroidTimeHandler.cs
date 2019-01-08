using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Timeinator.Mobile.Droid
{
    /// <summary>
    /// Manager to handle one session - Android extension
    /// </summary>
    public class AndroidTimeHandler : UserTimeHandler
    {
        #region Private members

        private TaskIntentService CurrentTaskSvc { get; set; }

        #endregion

        #region Interface implementation

        public override bool TimerStateRunning()
        {
            return CurrentTaskSvc.IsRunning();
        }

        public override void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            CurrentTaskSvc = new TaskIntentService(this);
            //Application.Context.BindService(); // if exists
            base.StartTimeHandler(sessionTasks);
            TaskTimer.Dispose();
            TaskTimer = new Timer { AutoReset = false };
        }

        public override void StartTask()
        {
            base.StartTask();
            var intent = new Intent(Application.Context, typeof(TaskIntentService));
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) // Depracated since API 26
                CurrentTaskSvc.StartService(intent);
            else
                CurrentTaskSvc.StartForegroundService(intent);
            CurrentTaskSvc.StartForeground(TaskIntentService.NOTIFICATION_ID, CurrentTaskSvc.GetNotification());
        }

        public override void StopTask()
        {
            base.StopTask();
            CurrentTaskSvc.StopSelf();
        }

        public override void ResumeTask()
        {
            base.ResumeTask();
            CurrentTaskSvc.StartForegroundService(new Intent(CurrentTaskSvc, typeof(TaskIntentService)));
            CurrentTaskSvc.StartForeground(TaskIntentService.NOTIFICATION_ID, CurrentTaskSvc.GetNotification());
        }

        #endregion
    }
}