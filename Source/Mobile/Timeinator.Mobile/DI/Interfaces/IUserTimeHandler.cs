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
        double RecentProgress { get; set; }

        void StartTimeHandler(List<TimeTaskContext> sessionTasks);
        List<TimeTaskContext> DownloadSession();
        void RefreshTasksState(ITimeTasksService mTimeTasksService);
        void RemoveAndContinueTasks(ITimeTasksService mTimeTasksService);
        TimeSpan TimeLossValue();
        void StartTask();
        void StopTask();
        void ResumeTask();
        void FinishTask();

        event Action TimesUp;
    }
}
