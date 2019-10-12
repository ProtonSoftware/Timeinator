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

        /// <summary>
        /// Tells whether service is up and running
        /// </summary>
        public bool Active
        {
            get
            {
                if (!IsConnected)
                    return false;

                return Binder.Service.IsRunning;
            }
        }

        /// <summary>
        /// Event to handle any request from notification
        /// </summary>
        public event Action<AppAction> Request = (a) => { };

        /// <summary>
        /// Update notification title text
        /// </summary>
        public void SetTitle(string txt)
        {
            if (IsConnected)
                Binder.Service.Title = txt;
        }

        /// <summary>
        /// Update notification title text
        /// </summary>
        public void SetProgress(double p)
        {
            if (IsConnected)
                Binder.Service.Progress = p;
        }

        /// <summary>
        /// Update notification title text
        /// </summary>
        public void SetTime(TimeSpan t)
        {
            if (IsConnected)
                Binder.Service.Time = t;
        }

        /// <summary>
        /// Repaint notification
        /// </summary>
        public void Update()
        {
            if (!IsConnected)
                return;
            Binder.Service.ReNotify();
        }

        /// <summary>
        /// Set state of Service to paused
        /// </summary>
        public void Stop()
        {
            if (!IsConnected)
                return;
            Binder.Service.IsRunning = false;
        }

        /// <summary>
        /// Set state of Service to running
        /// </summary>
        public void Start()
        {
            if (!IsConnected)
                return;
            Binder.Service.IsRunning = true;
        }

        /// <summary>
        /// Set state of Service
        /// </summary>
        public void SetState(bool running)
        {
            if (!IsConnected)
                return;
            Binder.Service.IsRunning = running;
        }

        /// <summary>
        /// Remove timeinator service
        /// </summary>
        public void Kill()
        {
            if (!IsConnected)
                return;
            Binder.Service.StopSelf();
        }
    }
}