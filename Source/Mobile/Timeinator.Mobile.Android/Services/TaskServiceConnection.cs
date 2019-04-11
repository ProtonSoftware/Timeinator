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
    /// Handles communication with Service using low-level TaskService and high-level <see cref="ISessionService"/>
    /// </summary>
    public class TaskServiceConnection : Java.Lang.Object, IServiceConnection, ISessionService
    {
        #region Constructors

        public TaskServiceConnection()
        {
            Active = false;
            TimerElapsed = () => { };
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
                Binder.Service.Elapsed += () => TimerElapsed.Invoke();
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

        public bool Active { get; private set; }
        public event Action TimerElapsed;

        public void Details(string nameT, double progressT)
        {
            TaskName = nameT;
            RecentProgress = progressT;
        }

        public void Interval(TimeSpan assignedT)
        {
            Active = false;
            if (!IsConnected)
                return;
            Binder.Service.Stop();
            Binder.Service.UpdateTask(TaskName, DateTime.Now, assignedT, RecentProgress);
        }

        public void Stop()
        {
            Active = false;
            if (!IsConnected)
                return;
            Binder.Service.Stop();
        }

        public void Start()
        {
            if (!IsConnected)
            {
                Active = false;
                return;
            }
            Binder.Service.ParamStart = DateTime.Now;
            Binder.Service.Start();
            Active = true;
        }

        public void Kill()
        {
            Active = false;
            if (!IsConnected)
                return;
            Binder.Service.KillService();
        }

        #endregion
    }
}