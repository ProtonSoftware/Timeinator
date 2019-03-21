using System;
using System.Collections.Generic;
using System.Text;
using System.Timers;

namespace Timeinator.Mobile.Core
{
    public interface IUserTimeHandler
    {
        TimeTaskContext CurrentTask { get; }
        TimeSpan TimePassed { get; }
        DateTime CurrentTaskStartTime { get; }
        double RecentProgress { get; set; }

        void StartTimeHandler(List<TimeTaskContext> sessionTasks);
        List<TimeTaskContext> DownloadSession();
        void RefreshTasksState();
        void RemoveAndContinueTasks();
        TimeSpan TimeLossValue();
        bool TimerStateRunning();
        void StartTask();
        void StopTask();
        void ResumeTask();
        void FinishTask();
        void InvokeTimesUp();

        event Action TimesUp;
    }
}
