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

        private TaskServiceConnection mServiceConnection;

        #endregion

        #region Interface implementation

        public override bool TimerStateRunning()
        {
            return mServiceConnection.IsConnected;
        }

        public override void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            base.StartTimeHandler(sessionTasks);
            //TaskTimer.Dispose();
            //TaskTimer = new Timer { AutoReset = false };
            ConnectService();
        }

        public override void StartTask()
        {
            base.StartTask();
        }

        public override void StopTask()
        {
            base.StopTask();
        }

        public override void ResumeTask()
        {
            base.ResumeTask();
        }

        public override void RemoveAndContinueTasks()
        {
            base.RemoveAndContinueTasks();
        }

        #endregion

        public void ConnectService()
        {
            if (mServiceConnection == null)
                mServiceConnection = new TaskServiceConnection();
            var intent = new Intent(Application.Context, typeof(TaskService));
            Application.Context.BindService(intent, mServiceConnection, Bind.AutoCreate);
            // Provide DI to service
            mServiceConnection.RefreshService(this);
        }
    }
}