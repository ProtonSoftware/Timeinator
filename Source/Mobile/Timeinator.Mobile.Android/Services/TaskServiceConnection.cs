using Android.App;
using Android.Content;
using Android.OS;
using System;
using Timeinator.Mobile.Domain;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Handles communication with Service using low-level TaskService
    /// </summary>
    public class TaskServiceConnection : Java.Lang.Object, IServiceConnection
    {
        #region Public Properties

        /// <summary>
        /// Indicates if this service is successfully connected and ready to use
        /// </summary>
        public bool IsConnected { get; private set; }
        
        /// <summary>
        /// The Android Binder service which is only used because all the services should run in background
        /// </summary>
        public TaskServiceBinder Binder { get; private set; }

        /// <summary>
        /// The event to handle any request coming from notification
        /// </summary>
        public event Action<AppAction> Request = (a) => { };

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Called initially when this service is created
        /// </summary>
        public void OnServiceConnected(ComponentName name, IBinder service)
        {
            // Get provided android binder
            Binder = service as TaskServiceBinder;

            // Connect this service if binder was provided
            IsConnected = Binder != null;
            if (IsConnected)
            {
                Binder.Service.RequestHandler += (a) => Request.Invoke(a);
            }
        }

        /// <summary>
        /// This is not called anywhere, just needed for <see cref="IServiceConnection"/> interface implementation
        /// </summary>
        public void OnServiceDisconnected(ComponentName name)
        {
            Binder = null;
            IsConnected = false;
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Updates notification data with current task state provided in parameters
        /// </summary>
        /// <param name="title">The task's name</param>
        /// <param name="progress">The progress of a task to show</param>
        /// <param name="time">The time left</param>
        /// <param name="runningState">The state of session</param>
        /// <returns></returns>
        public void UpdateTaskData(string title, double progress, TimeSpan time, bool runningState)
        {
            // Make sure we are connected
            if (!IsConnected)
                return;
            
            // Update all the data in notification service
            Binder.Service.Title = title;
            Binder.Service.Progress = (int)progress * 100;
            Binder.Service.Time = time;
            Binder.Service.IsRunning = runningState;
            Binder.Service.ReNotify();
        }

        /// <summary>
        /// Concludes the service by sending stop intent action to the application
        /// </summary>
        public void Kill()
        {
            // Make sure we are connected
            if (!IsConnected)
                return;

            // Create intent with stop action
            var intent = new Intent(Application.Context, typeof(TaskService));
            intent.SetAction(IntentActions.ACTION_STOP);

            // Send intent to the application
            Application.Context.StartService(intent);

            // Disconnect this service
            IsConnected = false;
        }

        #endregion
    }
}