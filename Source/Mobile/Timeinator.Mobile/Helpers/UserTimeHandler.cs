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


        private new Timer TaskTimer = new Timer();
        
        public void StartTask(TimeTaskContext currentTask)
        {
            CurrentTaskStartTime = DateTime.Now;
            TaskTimer.Interval = CurrentTask.AssignedTime.TotalMilliseconds;
            TaskTimer.Enabled = true;
        }

        public void StopTask()
        {
            TaskTimer.Enabled = false;
            CurrentTask.AssignedTime = DateTime.Now - CurrentTaskStartTime;
        }

        public void ResumeTask(TimeSpan TimeLeft)
        {

        }
            
        public void EndTask()
        {
            TaskTimer.Enabled = false;

        }

        private void SaveProgress()
        {
            CurrentTask.Progress = 
        }
}
