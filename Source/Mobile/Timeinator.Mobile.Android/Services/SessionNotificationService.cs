using Android.App;
using Android.Content;
using System;
using Timeinator.Mobile.Core;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The service to handle all notification interactions in a session
    /// </summary>
    public class SessionNotificationService
    {
        #region Private Members

        /// <summary>
        /// The main service connection for handling notification
        /// </summary>
        private readonly TaskServiceConnection mTaskServiceConnection = new TaskServiceConnection();

        /// <summary>
        /// Handler to communicate with and control session status
        /// </summary>
        private ISessionHandler mSessionHandler;

        /// <summary>
        /// Correct task name set flag
        /// </summary>
        private bool SuccesfulUpdate { get; set; } = false;

        #endregion

        #region Constructor

        public SessionNotificationService(ISessionHandler sessionHandler)
        {
            mSessionHandler = sessionHandler;

            mSessionHandler.SessionStarted += Initialise;
        }

        #endregion

        /// <summary>
        /// Setups this service by connecting with the <see cref="TaskServiceConnection"/>
        /// </summary>
        private void Initialise()
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

            // Attach provided notification interaction action
            mTaskServiceConnection.Request += NotificationRequest;

            // Init communication with handler
            mSessionHandler.SetupSession(TickNotification);
            mSessionHandler.SessionFinished += () => { mTaskServiceConnection.Kill(); };
            mSessionHandler.TaskStarted += TaskNotification;

            // Update task name
            TaskNotification();
        }

        /// <summary>
        /// Notification operations every tick
        /// </summary>
        private void TickNotification()
        {
            // Get state
            var paused = mSessionHandler.Paused;
            // Get progress
            var progress = mSessionHandler.CurrentTaskCalculatedProgress;
            // Get time left
            var time = mSessionHandler.CurrentTimeLeft;

            // Try to set task name if previous attempt failed
            if (!SuccesfulUpdate)
                TaskNotification();

            // Apply changes
            mTaskServiceConnection.SetState(!paused);
            mTaskServiceConnection.SetProgress(progress);
            mTaskServiceConnection.SetTime(time);
            mTaskServiceConnection.Update();
        }

        /// <summary>
        /// Notification operations every task
        /// </summary>
        private void TaskNotification()
        {
            // Update title
            var name = mSessionHandler.GetCurrentTask().Name;
            SuccesfulUpdate = mTaskServiceConnection.SetTitle(name);
        }

        /// <summary>
        /// Called when user interacted with session notification
        /// </summary>
        /// <param name="action">The action user has made</param>
        private void NotificationRequest(AppAction action)
        {
            // Fire proper command based on the action
            // So clicking on the notification has the exact same effect as clicking on the page
            switch (action)
            {
                case AppAction.NextSessionTask:
                    {
                        mSessionHandler.Finish();
                    } break;
                case AppAction.PauseSession:
                    {
                        mSessionHandler.Pause();
                    } break;
                case AppAction.ResumeSession:
                    {
                        mSessionHandler.Resume();
                    } break;
                case AppAction.StopSession:
                    {
                        mSessionHandler.EndSession();
                    } break;
            }
        }
    }
}