using Android.App;
using Android.Content;
using System.Threading.Tasks;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Handler for session - Android extension
    /// </summary>
    public class AndroidTimeHandler : UserTimeHandler
    {
        #region Private Members

        private TaskServiceConnection mServiceConnection;

        #endregion

        #region Private Helpers

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