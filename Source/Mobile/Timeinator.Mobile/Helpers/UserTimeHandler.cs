using System;
using System.Collections.Generic;
using System.Timers;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Manager that handles time within one user session
    /// </summary>
    public class UserTimeHandler
    {
        public List<TimeTaskContext> SessionTasks { get; set; }
        public DateTime CurrentTime => DateTime.Now;
        public DateTime CurrentTaskStartTime { get; set; }
        public TimeSpan TimeLeft => CurrentTask.AssignedTime - (CurrentTime - CurrentTaskStartTime);
        public TimeTaskContext CurrentTask => SessionTasks[0] ?? null;
        public event Action TimesUp;

        public void StartTask()
        {
            CurrentTaskStartTime = CurrentTime;
            TimeLeft 
        }

        public void StopTask()
        {

        }

        public void EndTask()
        {

        }

        public void SaveProgress()
        {

        }
}
