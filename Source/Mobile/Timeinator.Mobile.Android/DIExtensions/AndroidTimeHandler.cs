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

        private TaskServiceConnection ServiceConnection { get; set; }

        #endregion

        #region Interface implementation

        public override bool TimerStateRunning()
        {
            return ServiceConnection.IsRunning();
        }

        public override void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            //Application.Context.BindService(); // if exists
            base.StartTimeHandler(sessionTasks);
            TaskTimer.Dispose();
            TaskTimer = new Timer { AutoReset = false };
        }

        public override void StartTask()
        {
            base.StartTask();
            var intent = new Intent(Application.Context, typeof(TaskService));
            if (Build.VERSION.SdkInt < BuildVersionCodes.O) // Deprecated since API 26
                Application.Context.StartService(intent);
            else
                Application.Context.StartForegroundService(intent);
        }

        public override void StopTask()
        {
            base.StopTask();
            ServiceConnection.StopTaskService();
        }

        public override void ResumeTask()
        {
            base.ResumeTask();
        }

        #endregion
    }
}