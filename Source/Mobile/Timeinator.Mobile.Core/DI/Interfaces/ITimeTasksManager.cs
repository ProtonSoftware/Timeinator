using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile.Core
{
    public interface ITimeTasksManager
    {
        void UploadTasksList(List<TimeTaskContext> contexts, TimeSpan userTime);
        void UploadTime(TimeSpan userTime);
        TimeSpan GetMinimumTime();

        List<TimeTaskContext> GetCalculatedTasksListForSpecifiedTime();
    }
}
