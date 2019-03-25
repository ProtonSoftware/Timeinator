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

namespace Timeinator.Mobile.Droid
{
    /// <summary>
    /// Handler for session - Android extension
    /// </summary>
    public class AndroidTimeHandler : UserTimeHandler
    {
        #region Private members

        private TaskServiceConnection mServiceConnection;

        #endregion

        #region Interface implementation
        // SURPRISE! THERE IS NO INTERFACE IMPLEMENTATION NEEDED ANYMORE
        #endregion

        #region Private helpers

        private void ConnectService()
        {
            if (mServiceConnection == null)
                mServiceConnection = new TaskServiceConnection();
            var intent = new Intent(Application.Context, typeof(TaskService));
            Application.Context.BindService(intent, mServiceConnection, Bind.AutoCreate);
        }

        protected override void StartService()
        {
            ConnectService();
            mSessionService = mServiceConnection;
        }

        #endregion
    }
}