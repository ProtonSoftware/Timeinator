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

        private readonly ITimeTasksService mTimeTasksService;

        #endregion

        public AndroidTimeHandler(ITimeTasksService timeTasksService)
        {
            mTimeTasksService = timeTasksService;
        }

        #region Interface implementation

        public override bool TimerStateRunning()
        {
            return mCurrentTaskSvc.IsRunning();
        }

        public override void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            base.StartTimeHandler(sessionTasks);
            TaskTimer.Dispose();
            TaskTimer = new Timer { AutoReset = false };
            if (mCurrentTaskSvc != null)
                mCurrentTaskSvc.TaskServiceStop();
            mCurrentTaskSvc = new TaskIntentService(this, mTimeTasksService);
        }

        public override void StartTask()
        {
            base.StartTask();
            mCurrentTaskSvc.TaskServiceStart(CurrentTask.AssignedTime);
        }

        public override void StopTask()
        {
            base.StopTask();
            mCurrentTaskSvc.TaskServiceStop();
        }

        public override void ResumeTask()
        {
            base.ResumeTask();
            mCurrentTaskSvc.TaskServiceStart(CurrentTask.AssignedTime);
        }

        #endregion
    }
}