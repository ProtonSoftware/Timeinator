using Android.App;
using Android.Content;
using Timeinator.Mobile.Domain;

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
        /// The session handler that this service communicate with to proceed with session
        /// </summary>
        private readonly ISessionHandler mSessionHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public SessionNotificationService(ISessionHandler sessionHandler)
        {
            // Inject DI services
            mSessionHandler = sessionHandler;

            // Listen out for session start
            mSessionHandler.SessionStarted += Initialize;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Initializes this service for a start of new session
        /// </summary>
        private void Initialize()
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

            // Initialize communication with handler
            mSessionHandler.SetupSession(TaskUpdateTick);

            // Listen out for session finish event
            mSessionHandler.SessionFinished += mTaskServiceConnection.Kill;
        }

        /// <summary>
        /// Called whenever anything related to current task updates, so we can update all the data in notification
        /// </summary>
        private void TaskUpdateTick()
        {
            // Get all the data for current task
            var title = mSessionHandler.GetCurrentTask().Name;
            var paused = mSessionHandler.Paused;
            var progress = mSessionHandler.CurrentTaskCalculatedProgress;
            var time = mSessionHandler.CurrentTimeLeft;

            // Apply any changes
            mTaskServiceConnection.UpdateTaskData(title, progress, time, !paused);
        }

        /// <summary>
        /// Called when user interacted with session notification
        /// </summary>
        /// <param name="action">The action user has made</param>
        private void NotificationRequest(AppAction action)
        {
            // Fire proper function from handler based on user's action
            switch (action)
            {
                case AppAction.NextSessionTask:
                    {
                        mSessionHandler.Finish();
                    } 
                    break;
                case AppAction.PauseSession:
                    {
                        mSessionHandler.Pause();
                    } 
                    break;
                case AppAction.ResumeSession:
                    {
                        mSessionHandler.Resume();
                    }
                    break;
                case AppAction.StopSession:
                    {
                        mSessionHandler.EndSession();
                    }
                    break;
            }
        }

        #endregion
    }
}