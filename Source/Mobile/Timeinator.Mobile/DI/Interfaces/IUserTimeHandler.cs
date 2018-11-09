using System;
using System.Collections.Generic;
using System.Text;

namespace Timeinator.Mobile
{
    public interface IUserTimeHandler
    {
        void StartTimeHandler(List<TimeTaskContext> sessionTasks);
        void StartTask();
        void StopTask();
        void ResumeTask();
    }
}
