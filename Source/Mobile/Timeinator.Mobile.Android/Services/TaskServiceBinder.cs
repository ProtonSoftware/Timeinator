using Android.OS;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// The Android Binder containing background service
    /// </summary>
    public class TaskServiceBinder : Binder
    {
        #region Public Properties

        /// <summary>
        /// The actual service that runs in the background thanks to this binder
        /// </summary>
        public TaskService Service { get; private set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TaskServiceBinder(TaskService service)
        {
            Service = service;
        }

        #endregion
    }
}