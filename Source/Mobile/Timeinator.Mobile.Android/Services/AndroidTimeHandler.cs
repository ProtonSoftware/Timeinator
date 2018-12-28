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
            TaskTimer = new Timer
            {
                AutoReset = false
            };
            SessionTasks = new List<TimeTaskContext>(sessionTasks);
            CurrentTaskStartTime = CurrentTime;
            StartTask();
        }

        public override void StartTask()
        {
            base.StartTask();
        }

        #endregion
    }
}