using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile
{
    public interface INotificationHandler
    {
        void UploadTasksList(List<TimeTaskContext> contexts, TimeSpan userTime);

        List<TimeTaskContext> GetCalculatedTasksListForSpecifiedTime();
    }
}
