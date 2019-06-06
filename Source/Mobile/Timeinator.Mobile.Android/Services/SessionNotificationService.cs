using Android.App;
using Android.Content;
using System;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The service to handle all notification interactions in a session
    /// </summary>
    public class SessionNotificationService : ISessionNotificationService
    {
        #region Private Members

        /// <summary>
        /// The main service connection for handling notification
        /// </summary>
        private readonly TaskServiceConnection mTaskServiceConnection = new TaskServiceConnection();

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Setups this service by connecting with the <see cref="TaskServiceConnection"/>
        /// </summary>
        public void Setup()
        {
            // If the service is already connected...
            if (mTaskServiceConnection.IsConnected)
                // Don't do anything
                return;

            // Create new intent for that service
            var intent = new Intent(Application.Context, typeof(TaskService));

            // Setup android foreground service
            Application.Context.StartService(intent);
            Application.Context.BindService(intent, mTaskServiceConnection, Bind.WaivePriority);
        }

        /// <summary>
        /// Attaches provided action to the notification interactions
        /// </summary>
        /// <param name="notificationButtonClick">The action to fire whenever notification interaction happens</param>
        public void AttachClickCommands(Action<AppAction> notificationButtonClick)
        {
            // Attach provided notification interaction action
            mTaskServiceConnection.Request += notificationButtonClick;
        }

        /// <summary>
        /// Starts new task in the notification
        /// </summary>
        /// <param name="timeTaskViewModel">The provided task's view model to start</param>
        public void StartNewTask(TimeTaskViewModel timeTaskViewModel)
        {
            // Stop any previous tasks
            mTaskServiceConnection.Stop();

            // Setup provided details
            mTaskServiceConnection.Details(timeTaskViewModel.Name, timeTaskViewModel.Progress);
            mTaskServiceConnection.Interval(timeTaskViewModel.AssignedTime);

            // Start the task
            mTaskServiceConnection.Start();
        }

        /// <summary>
        /// Stops current task
        /// </summary>
        public void StopCurrentTask() => mTaskServiceConnection.Stop();

        /// <summary>
        /// Removes the notification from the phone
        /// </summary>
        public void RemoveNotification() => mTaskServiceConnection.Kill();

        /// <summary>
        /// Updates notification state
        /// </summary>
        public void UpdateNotification() => mTaskServiceConnection.Update();

        #endregion
    }
}