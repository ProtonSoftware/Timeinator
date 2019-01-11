﻿using System;
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
    public class TaskServiceConnection : Java.Lang.Object, IServiceConnection, ITaskService
    {
        public TaskServiceConnection()
        {
            IsConnected = false;
            Binder = null;
        }

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

        #region Interface Implementation

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

        public void RefreshService(IUserTimeHandler androidTimeHandler)
        {
            if (!IsConnected)
                return;
            Binder.Service.RefreshService(androidTimeHandler);
        }

        #endregion
    }
}