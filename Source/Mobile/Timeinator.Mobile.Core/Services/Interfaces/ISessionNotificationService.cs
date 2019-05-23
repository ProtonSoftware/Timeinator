using System;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for a service that handles all the notification interactions within session
    /// </summary>
    public interface ISessionNotificationService
    {
        void Setup();
        void AttachClickCommands(Action<AppAction> notificationButtonClick);

        void StartNewTask(TimeTaskViewModel timeTaskViewModel);
        void StopCurrentTask();
        void RemoveNotification();
    }
}
