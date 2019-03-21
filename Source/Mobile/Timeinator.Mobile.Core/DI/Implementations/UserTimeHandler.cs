using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Manager that handles time within one user session
    /// </summary>
    public class UserTimeHandler : IUserTimeHandler
    {
        #region Public Properties

        /// <summary>
        /// List of tasks for one session
        /// </summary>
        public List<TimeTaskContext> SessionTasks { get; set; }

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
        /// Returns time that passed from the beginning
        /// </summary>
        public TimeSpan TimePassed => CurrentTime.Subtract(CurrentTaskStartTime);

        /// <summary>
        /// Stores progress that passed before pausing task
        /// </summary>
        public double RecentProgress { get; set; }

        /// <summary>
        /// Event called when time for task elapsed
        /// </summary>
        public event Action TimesUp;

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Resets handler and loads list of tasks to TimeHandler and starts the first one
        /// </summary>
        /// <param name="sessionTasks">Session tasks sorted by OrderId</param>
        public virtual void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            TaskTimer.Dispose();
            TaskTimer = new Timer { AutoReset = false };
            TaskTimer.Elapsed += (sender, e) => TimesUp.Invoke();
            SessionTasks = new List<TimeTaskContext>(sessionTasks);
            CurrentTaskStartTime = CurrentTime;
            StartTask();
        }

        /// <summary>
        /// Function serving correct Session Tasks
        /// </summary>
        public virtual List<TimeTaskContext> DownloadSession()
        {
            return SessionTasks;
        }

        /// <summary>
        /// Recalculate assigned times on current session
        /// </summary>
        public void RefreshTasksState()
        {
            var ttsvc = Dna.Framework.Service<ITimeTasksService>();
            ttsvc.ConveyTasksToManager(SessionTasks);
            SessionTasks = ttsvc.GetCalculatedTasksFromManager();
        }

        /// <summary>
        /// Pushes tasks forward on stack if CurrentTask is finished
        /// </summary>
        public virtual void RemoveAndContinueTasks()
        {
            if (CurrentTask == null || CurrentTask.Progress < 1)
                return;
            TaskTimer.Stop();
            var ttsvc = Dna.Framework.Service<ITimeTasksService>();
            ttsvc.RemoveFinishedTasks(new List<TimeTaskContext> { CurrentTask });
            SessionTasks.Remove(CurrentTask);
            StartTask();
        }

        /// <summary>
        /// Gets CurrentTask time loss every second of Break
        /// </summary>
        public virtual TimeSpan TimeLossValue()
        {
            if (CurrentTask == null)
                return default(TimeSpan);
            return TimeSpan.FromSeconds((int)CurrentTask.Priority / SessionTasks.SumPriorities());
        }

        /// <summary>
        /// Checks state of TaskTimer
        /// </summary>
        public virtual bool TimerStateRunning() => TaskTimer.Enabled;

        /// <summary>
        /// Starts next task
        /// </summary>
        public virtual void StartTask()
        {
            TaskTimer.Stop();
            if (CurrentTask == null)
                return;
            var CurrentTaskAssignedMilliseconds = CurrentTask.AssignedTime.TotalMilliseconds;
            if (CurrentTaskAssignedMilliseconds > 0)
            {
                RecentProgress = 0;
                TaskTimer.Interval = CurrentTaskAssignedMilliseconds;
                CurrentTaskStartTime = CurrentTime;
                TaskTimer.Start();
            }
            else
                SessionTasks.Remove(CurrentTask);
        }

        /// <summary>
        /// Stops the task and saves its progress
        /// </summary>
        public virtual void StopTask()
        {
            TaskTimer.Stop();
            if (CurrentTask == null)
                return;
            SaveProgress();
        }

        /// <summary>
        /// Resumes the task basing on progress of the task
        /// </summary>
        public virtual void ResumeTask()
        {
            if (CurrentTask == null)
                return;
            CurrentTaskStartTime = CurrentTime;
            var assignedms = CurrentTask.AssignedTime.TotalMilliseconds;
            if (assignedms > 0)
            {
                TaskTimer.Interval = CurrentTask.AssignedTime.TotalMilliseconds;
                TaskTimer.Start();
            }
        }

        /// <summary>
        /// Stops the timer and sets Current Task progress as finished
        /// </summary>
        public virtual void FinishTask()
        {
            if (CurrentTask == null)
                return;
            TaskTimer.Stop();
            CurrentTask.Progress = 1;
        }

        /// <summary>
        /// Call TimesUp
        /// </summary>
        public void InvokeTimesUp()
        {
            TimesUp.Invoke();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Timer used in all of tasks sessions
        /// </summary>
        protected Timer TaskTimer { get; set; } = new Timer();

        /// <summary>
        /// Stores current time
        /// </summary>
        protected DateTime CurrentTime => DateTime.Now;

        /// <summary>
        /// Method used to save progress of the task when it gets paused
        /// </summary>
        protected virtual void SaveProgress()
        {
            var step = TimePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds;
            CurrentTask.Progress = RecentProgress + (1.0 - RecentProgress) * step;
            RecentProgress += step;
        }

        #endregion
    }
}
