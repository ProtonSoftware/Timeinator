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
    /// Handles communication with Service using low-level <see cref="ITaskService"/> and high-level <see cref="ISessionService"/>
    /// </summary>
    public class TaskServiceConnection : Java.Lang.Object, IServiceConnection, ITaskService, ISessionService
    {
        #region Constructors

        public TaskServiceConnection()
        {
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
        }

        public void OnServiceDisconnected(ComponentName name)
        {
            Binder = null;
            IsConnected = false;
        }

        #endregion

        #region ISessionService Implementation
        public bool Active { get; set; }
        public event Action TimerElapsed;
        public void Interval(TimeSpan assignedT) { }
        public void Stop() { }
        public void Start() { }
        /*
        public bool Active => IsRunning();

        public event Action TimerElapsed
        {
            add { Binder.Service.TimerElapsed += value; }
            remove { Binder.Service.TimerElapsed -= value; }
        }

        public void Interval(TimeSpan assignedT)
        {
            if (Active)
            {
                var intent = new Intent();
                intent.SetAction(IntentActions.ACTION_PAUSETASK);
                HandleMessage(intent);
            }
            if (IsConnected)
            {
                Binder.Service.Start = DateTime.Now;
                Binder.Service.Time = assignedT;
                Binder.Service.Name = "TEST";
                Binder.Service.RecentProgress = 0;
            }
        }

        public void Stop()
        {
            var intent = new Intent();
            intent.SetAction(IntentActions.ACTION_PAUSETASK);
            HandleMessage(intent);
        }

        public void Start()
        {
            var intent = new Intent();
            intent.SetAction(IntentActions.ACTION_RESUMETASK);
            HandleMessage(intent);
        }
        */
        #endregion

        #region ITaskService Implementation

        public Notification GetNotification()
        {
            if (!IsConnected)
                return null;
            return Binder.Service.GetNotification();
        }

        public void HandleMessage(Intent intent)
        {
            if (!IsConnected)
                return;
            Binder.Service.HandleMessage(intent);
        }

        public void StopTaskService()
        {
            if (!IsConnected)
                return;
            Binder.Service.StopTaskService();
        }

        public bool IsRunning()
        {
            if (!IsConnected)
                return false;
            return Binder.Service.IsRunning();
        }

        #endregion
    }
}