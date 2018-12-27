using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace Timeinator.Mobile.Droid
{
    [Service]
    public class TaskIntentService : IntentService
    {
        private int SleepTime { get; set; }

        public TaskIntentService(int assignedTime) : base("TaskIntentService")
        {
            SleepTime = assignedTime;
        }

        protected override void OnHandleIntent(Intent intent)
        {
            Thread.Sleep(SleepTime);
        }
    }
}