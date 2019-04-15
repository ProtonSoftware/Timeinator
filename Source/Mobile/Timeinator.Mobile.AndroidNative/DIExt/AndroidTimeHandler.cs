using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Timers;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.AndroidNative
{
    /// <summary>
    /// Handler for session - Android extension
    /// </summary>
    public class AndroidTimeHandler : UserTimeHandler
    {
        #region Private members

        private TaskServiceConnection mServiceConnection;

        #endregion

        #region Private helpers

        private void ConnectService()
        {
            if (mServiceConnection == null)
                mServiceConnection = new TaskServiceConnection();
            if (mServiceConnection.IsConnected)
                return;
            var intent = new Intent(Application.Context, typeof(TaskService));
            Application.Context.StartService(intent);
            Application.Context.BindService(intent, mServiceConnection, Bind.WaivePriority);
        }

        protected override void StartService()
        {
            ConnectService();
            mSessionService = mServiceConnection;
        }

        #endregion
    }
}