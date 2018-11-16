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
            TaskTimer.Elapsed += (sender, e) => TimesUp.Invoke();
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

            var CurrentTaskAssignedMilliseconds = CurrentTask.AssignedTime.TotalMilliseconds;
            if (CurrentTaskAssignedMilliseconds > 0)
            { 
                TaskTimer.Interval = CurrentTaskAssignedMilliseconds;
                TaskTimer.Enabled = true;
            }

            else
            {
                SessionTasks.Remove(CurrentTask);
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="response"> True if user wants to start a new task, False if they want to take a break </param>
        public void IfStartUserResponse(bool response)
        {
            if (response == true)
            {
                StartTask();
            }
            else
            {

            }
        }

        /// <summary>
        /// Catches user response after asking whether task is finished
        /// </summary>
        /// <param name="response"></param>
        public void FinishedTaskUserResponse(bool response)
        {
            if (response == false)
            {
                SessionTasks.Remove(CurrentTask);
            }
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
        /// Activated if task completed ahead of schedule
        /// </summary>
        public void FinishTask()
        {
            TaskTimer.Enabled = false;
            CurrentTask.Progress = 1;
            SessionTasks.Remove(CurrentTask);
        }

        /// <summary>
        /// Method used to save progress of the task
        /// </summary>
        private void SaveProgress()
        {
            CurrentTask.Progress += TimePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds;
        }
    }
}
