using System;
using System.Collections.Generic;
using System.Timers;

namespace Timeinator.Mobile
{
    /// <summary>
    /// Manager that handles time within one user session
    /// </summary>
    public class UserTimeHandler : IUserTimeHandler
    {
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
        public TimeTaskContext CurrentTask => SessionTasks[0] ?? null;
        /// <summary>
        /// Timer used in all of tasks sessions
        /// </summary>
        public Timer TaskTimer { get; set; } = new Timer();
        /// <summary>
        /// Event called when time for task elapsed
        /// </summary>
        public event Action TimesUp;

        /// <summary>
        /// Loads list of tasks to TimeHandler and starts the first one
        /// </summary>
        /// <param name="sessionTasks"></param>
        public void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            SessionTasks = sessionTasks;
            StartTask();
        }

        /// <summary>
        /// Starts the task
        /// </summary>
        public void StartTask()
        {
            CurrentTaskStartTime = CurrentTime;
            TaskTimer.Interval = CurrentTask.AssignedTime.TotalMilliseconds;
            TaskTimer.Elapsed += (sender, e) => TimesUp.Invoke();
            TaskTimer.Enabled = true;
        }

        /// <summary>
        /// Stops the task and removes if completed
        /// </summary>
        public void StopTask()
        {
            TaskTimer.Enabled = false;
            SaveProgress();
            if (CurrentTask.Progress >= 1)
                SessionTasks.Remove(CurrentTask);
        }

        /// <summary>
        /// Resumes the task basing on progress of the task
        /// </summary>
        public void ResumeTask()
        {
            CurrentTaskStartTime = CurrentTime;
            TaskTimer.Interval = (1 - CurrentTask.Progress) * CurrentTask.AssignedTime.TotalMilliseconds;
            TaskTimer.Enabled = true;
        }

        /// <summary>
        /// Changes task to a constant time specified by user and recalculates times
        /// </summary>
        public void ExtendTask()
        {
            //TODO: Michał napraw to żeby przedłużało czas zadania i przeliczało managerem od nowa
        }

        /// <summary>
        /// Method used to save progress of the task
        /// </summary>
        private void SaveProgress()
        {
            var timePassed = CurrentTime.Subtract(CurrentTaskStartTime);
            CurrentTask.Progress += timePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds;
        }

        public void Reset()
        {
            //TODO(if necessary)
        }
    }
}
