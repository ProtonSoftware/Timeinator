using System;
using System.Collections.Generic;
using System.Linq;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// Manager that handles time within one user session
    /// </summary>
    public class UserTimeHandler : IUserTimeHandler
    {
        #region Protected Properties

        /// <summary>
        /// Interface for accessing Timer handling functionality
        /// </summary>
        protected ISessionService mSessionService;

        /// <summary>
        /// Stores current time
        /// </summary>
        protected DateTime CurrentTime => DateTime.Now;

        #endregion

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
        /// Tells whether Session Timer is running
        /// </summary>
        public bool SessionRunning => mSessionService.Active;

        #endregion

        #region Public Events

        /// <summary>
        /// Event called when time for task elapsed
        /// </summary>
        public event Action TimesUp
        {
            add { mSessionService.TimerElapsed += value; }
            remove { mSessionService.TimerElapsed -= value; }
        }

        /// <summary>
        /// Event called when Handler gets updated from background Request
        /// </summary>
        public event Action Updated;

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Resets handler, starts <see cref="ISessionService"/>, loads list of tasks to TimeHandler and starts the first one
        /// </summary>
        /// <param name="sessionTasks">Session tasks sorted by OrderId</param>
        public virtual void StartTimeHandler(List<TimeTaskContext> sessionTasks)
        {
            if (mSessionService != null)
                mSessionService.Kill();
            StartService();
            Updated = () => { };
            mSessionService.Request += SessionService_Request;
            SessionTasks = new List<TimeTaskContext>(sessionTasks);
            CurrentTaskStartTime = CurrentTime;
            StartTask();
        }

        /// <summary>
        /// Function providing correct Session Tasks
        /// </summary>
        public virtual List<TimeTaskContext> DownloadSession()
        {
            return SessionTasks;
        }

        /// <summary>
        /// Recalculate assigned times on current session
        /// </summary>
        public virtual void RefreshTasksState()
        {
            var ttsvc = Dna.Framework.Service<ITimeTasksService>();
            ttsvc.SetSessionTasks(SessionTasks);
            SessionTasks = ttsvc.GetCalculatedTasks();
        }

        /// <summary>
        /// Pushes tasks forward on stack if CurrentTask is finished
        /// </summary>
        public virtual void CleanTasks()
        {
            if (CurrentTask == null || CurrentTask.Progress < 1)
                return;
            mSessionService.Stop();
            var ttsvc = Dna.Framework.Service<ITimeTasksService>();
            ttsvc.RemoveFinishedTasks(new List<TimeTaskContext> { CurrentTask });
            SessionTasks.Remove(CurrentTask);
            if (CurrentTask == null)
                mSessionService.Kill();
        }

        /// <summary>
        /// Gets CurrentTask time loss every second of Break
        /// </summary>
        public TimeSpan TimeLossValue()
        {
            if (CurrentTask == null)
                return default;
            return TimeSpan.FromSeconds((int)CurrentTask.Priority / SessionTasks.SumPriorities());
        }

        /// <summary>
        /// Starts next task
        /// </summary>
        public virtual void StartTask()
        {
            mSessionService.Stop();
            if (CurrentTask == null)
                return;
            var CurrentTaskAssignedT = CurrentTask.AssignedTime;
            if (CurrentTaskAssignedT.TotalMilliseconds > 0)
            {
                RecentProgress = 0;
                mSessionService.Details(CurrentTask.Name, RecentProgress);
                mSessionService.Interval(CurrentTaskAssignedT);
                CurrentTaskStartTime = CurrentTime;
                mSessionService.Start();
            }
            else
                SessionTasks.Remove(CurrentTask);
        }

        /// <summary>
        /// Stops the task and saves its progress
        /// </summary>
        public virtual void StopTask()
        {
            mSessionService.Stop();
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
            var CurrentTaskAssignedT = CurrentTask.AssignedTime;
            if (CurrentTaskAssignedT.TotalMilliseconds > 0)
            {
                mSessionService.Details(CurrentTask.Name, RecentProgress);
                mSessionService.Interval(CurrentTaskAssignedT);
                mSessionService.Start();
            }
        }

        /// <summary>
        /// Stops the timer and sets Current Task progress as finished
        /// </summary>
        public virtual void FinishTask()
        {
            if (CurrentTask == null)
                return;
            mSessionService.Stop();
            CurrentTask.Progress = 1;
        }

        #endregion

        #region Protected Helpers

        /// <summary>
        /// Method used to save progress of the task when it gets paused
        /// </summary>
        protected virtual void SaveProgress()
        {
            var step = TimePassed.TotalMilliseconds / CurrentTask.AssignedTime.TotalMilliseconds;
            CurrentTask.Progress = RecentProgress + (1.0 - RecentProgress) * step;
            RecentProgress += step;
        }

        /// <summary>
        /// Get the proper Service running
        /// </summary>
        protected virtual void StartService()
        {
            if (mSessionService == null)
                mSessionService = new DrySessionService();
        }

        /// <summary>
        /// Handle action requested by Service
        /// </summary>
        protected virtual void SessionService_Request(AppAction obj)
        {
            if (obj == AppAction.NextSessionTask)
            {
                FinishTask();
                CleanTasks();
                RefreshTasksState();
                StartTask();
            }
            else if (obj == AppAction.PauseSession)
                StopTask();
            else if (obj == AppAction.ResumeSession)
            {
                RefreshTasksState();
                ResumeTask();
            }
            Updated.Invoke();
        }

        #endregion
    }
}
