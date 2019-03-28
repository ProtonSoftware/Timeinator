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
        #endregion
    }
}