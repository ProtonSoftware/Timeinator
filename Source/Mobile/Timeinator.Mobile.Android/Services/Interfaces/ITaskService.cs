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
    /// <summary>
    /// Interface for Android-specific way of handling service
    /// </summary>
    public interface ITaskService
    {
        Notification GetNotification();
        void HandleMessage(Intent intent);
        void StopTaskService();
        bool IsRunning();
    }
}