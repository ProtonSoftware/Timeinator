using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Timeinator.Mobile
{
    public interface IUserTimeHandler
    {
        Timer TaskTimer { get; set; }
        TimeSpan TimePassed { get; }

        void StartTimeHandler(List<TimeTaskContext> sessionTasks);
        List<TimeTaskContext> DownloadSession();
        void UpdateSession(List<TimeTaskContext> sessionTasks);
        void StartTask();
        void StopTask();
        void ResumeTask();
        void FinishTask();

        event Action TimesUp;
    }
}
