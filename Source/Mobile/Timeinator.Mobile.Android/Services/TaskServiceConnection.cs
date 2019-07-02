using Android.Content;
using Android.OS;
using System;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Handles communication with Service using low-level TaskService
    /// </summary>
    public class TaskServiceConnection : Java.Lang.Object, IServiceConnection
    {
        #region Public Properties

        public bool IsConnected { get; private set; }
        public TaskServiceBinder Binder { get; private set; }

        #endregion

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as TaskServiceBinder;
            IsConnected = Binder != null;
            if (IsConnected)
            {
                Binder.Service.RequestHandler += (a) => Request.Invoke(a);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Binder = null;
            IsConnected = false;
        }

        private string TaskName { get; set; }
        private double RecentProgress { get; set; }

        public bool Active
        {
            get
            {
                if (!IsConnected)
                    return false;

                return Binder.Service.IsRunning;
            }
        }

        public event Action<AppAction> Request = (a) => { };

        public void Details(string nameT, double progressT)
        {
            TaskName = nameT;
            RecentProgress = progressT;
        }

        public void Update()
        {
            if (!IsConnected)
                return;
            Binder.Service.ReNotify();
        }

        public void Interval(TimeSpan assignedT)
        {
            if (!IsConnected)
                return;
            Binder.Service.IsRunning = false;
            Binder.Service.UpdateTask(TaskName, assignedT, RecentProgress);
        }

        public void Stop()
        {
            if (!IsConnected)
                return;
            Binder.Service.IsRunning = false;
        }

        public void Start()
        {
            if (!IsConnected)
                return;
            Binder.Service.IsRunning = true;
        }

        public void Kill()
        {
            if (!IsConnected)
                return;
            Binder.Service.StopSelf();
        }
    }
}