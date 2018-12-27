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

namespace Timeinator.Mobile.Droid
{
    public class TasksServiceHandler : ITasksServiceHandler
    {
        #region Private members

        private TaskIntentService mCurrentTaskSvc;

        #endregion

        #region Interface implementation

        public void NewTask(int assignedTime)
        {
            mCurrentTaskSvc = new TaskIntentService(assignedTime);
            mCurrentTaskSvc.StartForegroundService(new Intent());
        }

        public void StopTask()
        {
            mCurrentTaskSvc.StopSelf();
        }

        #endregion
    }
}