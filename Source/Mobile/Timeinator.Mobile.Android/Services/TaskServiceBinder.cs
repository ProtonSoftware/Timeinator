using Android.OS;

namespace Timeinator.Mobile.Android
{
    public class TaskServiceBinder : Binder
    {
        public TaskServiceBinder(TaskService service)
        {
            Service = service;
        }

        public TaskService Service { get; private set; }
    }
}