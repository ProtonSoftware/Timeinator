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
    [Service]
    public class TaskIntentService : IntentService
    {
        private static Timer TaskTimer = new Timer();
        private TimeSpan TaskTime { get; set; }

        public TaskIntentService(DateTime assignedTime) : base("TaskIntentService")
        {
            TaskTime = DateTime.Now - assignedTime;
        }

        protected override void OnHandleIntent(Intent intent)
        {
            if (intent.Action == IntentActions.ACTION_NEXTTASK)
                return;
        }
    }
}