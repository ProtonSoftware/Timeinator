using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Manager that handles time within one user session
    /// </summary>
    public class UserTimeHandler : IUserTimeHandler
    {
        #region Public properties
        /// <summary>
        /// List of tasks for one session
        /// </summary>
        public List<TimeTaskContext> SessionTasks { get; set; }

        /// <summary>
        /// Stores current time
        /// </summary>
        public DateTime CurrentTime => DateTime.Now;

        /// <summary>
        /// Stores start time of the task
        /// </summary>
        public DateTime CurrentTaskStartTime { get; set; }

        /// <summary>
        /// Stores current task - first from the list (by default) 
        /// </summary>
        public TimeTaskContext CurrentTask {
            get
            {
                try { return SessionTasks.ElementAt(0); }
                catch { return null; }
            }
        }

        /// <summary>
        /// Timer used in all of tasks sessions
        /// </summary>
        public Timer TaskTimer { get; set; } = new Timer();

        /// <summary>
        /// Returns time that passed from the beginning
        /// </summary>
        public TimeSpan TimePassed => CurrentTime.Subtract(CurrentTaskStartTime);

        /// <summary>
        /// Stores time that passed before pausing task
        /// </summary>
        public TimeSpan RecentTimePassed { get; set; }

        /// <summary>
        /// Event called when time for task elapsed
        /// </summary>
        public event Action TimesUp;
        #endregion

        #region Interface implementation
        /// <summary>
        /// Resets handler and loads list of tasks to TimeHandler and starts the first one
        /// </summary>
        /// <param name="sessionTasks">Session tasks sorted by OrderId</param>
        public void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            TaskTimer.Dispose();
            TaskTimer = new Timer
            {
                AutoReset = false
            };
            TaskTimer.Elapsed += (sender, e) => TimesUp.Invoke();
            SessionTasks = new List<TimeTaskContext>(sessionTasks);
            CurrentTaskStartTime = CurrentTime;
            StartTask();
        }

        /// <summary>
        /// Updates session on runtime
        /// </summary>
        /// <param name="sessionTasks">Session tasks sorted by OrderId</param>
        public void UpdateSession(List<TimeTaskContext> sessionTasks)
        {
            TaskTimer.Stop();
            SessionTasks = new List<TimeTaskContext>(sessionTasks);
            StartTask();
        }

        /// <summary>
        /// Function serving correct Session Tasks
        /// </summary>
        public List<TimeTaskContext> DownloadSession()
        {
            return SessionTasks;
        }

        /// <summary>
        /// Starts next task
        /// </summary>
        public void StartTask()
        {
            TaskTimer.Stop();
            if (CurrentTask == null)
                return;

            var CurrentTaskAssignedMilliseconds = CurrentTask.AssignedTime.TotalMilliseconds;
            if (CurrentTaskAssignedMilliseconds > 0)
            {
                RecentTimePassed = new TimeSpan(0);
                TaskTimer.Interval = CurrentTaskAssignedMilliseconds;
                CurrentTaskStartTime = CurrentTime;
                TaskTimer.Start();
            }
            else
                SessionTasks.Remove(CurrentTask);
        }

        /// <summary>
        /// Stops the task and removes it if completed
        /// </summary>
        public void StopTask()
        {
            TaskTimer.Stop();
            if (CurrentTask == null)
                return;
            SaveProgress();
            if (CurrentTask.Progress >= 1)
                FinishTask();
        }

        /// <summary>
        /// Resumes the task basing on progress of the task
        /// </summary>
        public void ResumeTask()
        {
            if (CurrentTask == null)
                return;
            CurrentTaskStartTime = CurrentTime;
            TaskTimer.Interval = (1 - CurrentTask.Progress) * CurrentTask.AssignedTime.TotalMilliseconds;
            TaskTimer.Start();
        }

        /// <summary>
        /// Stops the timer and removes current task
        /// </summary>
        public void FinishTask()
        {
            if (CurrentTask == null)
                return;
            TaskTimer.Stop();
            CurrentTask.Progress = 1;
            SessionTasks.Remove(CurrentTask);
        }
        #endregion

        #region Private helpers
        /// <summary>
        /// Method used to save progress of the task when it gets paused
        /// </summary>
        private void SaveProgress()
        {
            CurrentTask.Progress = (RecentTimePassed.TotalMilliseconds + TimePassed.TotalMilliseconds) / CurrentTask.AssignedTime.TotalMilliseconds;
            RecentTimePassed += new TimeSpan(TimePassed.Ticks);
        }
        #endregion
    }
}
