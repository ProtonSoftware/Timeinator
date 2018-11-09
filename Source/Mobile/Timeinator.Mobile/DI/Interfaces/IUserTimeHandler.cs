using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Timeinator.Mobile
{
    public interface IUserTimeHandler
    {
        Timer TaskTimer { get; set; }

        void StartTimeHandler(List<TimeTaskContext> sessionTasks);
        void StartTask();
        void StopTask();
        void ResumeTask();
        void ExtendTask();
    }
}
