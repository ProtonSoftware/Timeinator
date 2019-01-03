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
    public class AndroidTimeHandler : UserTimeHandler
    {
        #region Private members

        private TaskIntentService mCurrentTaskSvc;

        #endregion

        #region Interface implementation

        public override void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            base.StartTimeHandler(sessionTasks);
            TaskTimer.Dispose();
            TaskTimer = new Timer { AutoReset = false };
        }

        public override void StartTask()
        {
            base.StartTask();
            mCurrentTaskSvc.TaskServiceStop();
            mCurrentTaskSvc = new TaskIntentService(CurrentTask.AssignedTime);
        }

        #endregion
    }
}