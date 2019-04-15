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

namespace Timeinator.Mobile.AndroidNative
{
    /// <summary>
    /// Handles communication with Service using low-level TaskService and high-level <see cref="ISessionService"/>
    /// </summary>
    public class TaskServiceConnection : Java.Lang.Object, IServiceConnection, ISessionService
    {
        #region Constructors

        public TaskServiceConnection()
        {
            TimerElapsed = () => { };
            Request = (a) => { };
            IsConnected = false;
            Binder = null;
        }

        #endregion

        #region Public properties

        public bool IsConnected { get; private set; }
        public TaskServiceBinder Binder { get; private set; }

        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            Binder = service as TaskServiceBinder;
            IsConnected = Binder != null;
            if (IsConnected)
            {
                Binder.Service.Elapsed += () => TimerElapsed.Invoke();
                Binder.Service.RequestHandler += (a) => Request.Invoke(a);
            }
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Binder = null;
            IsConnected = false;
        }

        #endregion

        #region SessionService Implementation

        private string TaskName { get; set; }
        private double RecentProgress { get; set; }

        public bool Active
        {
            get {
                if (!IsConnected)
                    return false;
                else
                    return Binder.Service.Running;
            }
        }
        public event Action TimerElapsed;
        public event Action<AppAction> Request;

        public void Details(string nameT, double progressT)
        {
            TaskName = nameT;
            RecentProgress = progressT;
        }

        public void Interval(TimeSpan assignedT)
        {
            if (!IsConnected)
                return;
            Binder.Service.Stop();
            Binder.Service.UpdateTask(TaskName, DateTime.Now, assignedT, RecentProgress);
        }

        public void Stop()
        {
            if (!IsConnected)
                return;
            Binder.Service.Stop();
        }

        public void Start()
        {
            if (!IsConnected)
                return;
            Binder.Service.ParamStart = DateTime.Now;
            Binder.Service.Start();
        }

        public void Kill()
        {
            if (!IsConnected)
                return;
            Binder.Service.KillService();
        }

        #endregion
    }
}