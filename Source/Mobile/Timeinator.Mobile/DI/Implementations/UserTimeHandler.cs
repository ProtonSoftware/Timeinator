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
        /// Returns time that passed from the beginning
        /// </summary>
        public TimeSpan TimePassed => CurrentTime.Subtract(CurrentTaskStartTime);
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
        /// Function serving correct Session Tasks
        /// </summary>
        public List<TimeTaskContext> DownloadSession()
        {
            //TO DO: make sure tasks are correctly served
            return SessionTasks;
        }

        /// <summary>
        /// Starts the task
        /// </summary>
        public void StartTask()
        {
            CurrentTaskStartTime = CurrentTime;
            // TODO: Michał, leci exception tutaj https://i.imgur.com/LN95nY8.png (nie twoja wina, ale mozesz zrobic jakies)
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

        public void FinishTask()
        {

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
            CurrentTask.Progress += TimePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds;
        }

        public void Reset()
        {
            //TODO(if necessary)
        }
    }
}
