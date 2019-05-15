using System;
using System.Collections.Generic;
using System.Timers;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The timer implementation for task session
    /// </summary>
    public class SessionTimer : ISessionTimer
    {
        #region Private Members

        /// <summary>
        /// The timer that elapses every second so everything related to time can update everytime it ticks
        /// </summary>
        public Timer mSecondsTicker;

        #endregion

        #region Public Properties

        /// <summary>
        /// The duration of the whole current session
        /// </summary>
        public TimeSpan SessionDuration { get; private set; } = new TimeSpan(0);

        /// <summary>
        /// The time that left in current session task
        /// </summary>
        public TimeSpan CurrentTaskTimeLeft { get; private set; }

        #endregion

        #region Public Events

        /// <summary>
        /// The event that is fired any time the current task finishes
        /// </summary>
        public event Action TaskFinished = () => { };

        #endregion

        #region Interface Implementation

        public void SetupSession(Action timerAction, Action taskAction)
        {
            // Timer ticks every second
            mSecondsTicker = new Timer(1000);

            // Run our elapsed function every time timer ticks
            mSecondsTicker.Elapsed += SecondsTicker_Elapsed;

            // Attach provided action as well
            mSecondsTicker.Elapsed += (s, e) => timerAction.Invoke();

            // Attach provided task action to the task finished event
            TaskFinished += taskAction;
        }

        /// <summary>
        /// Starts new task in the session
        /// </summary>
        /// <param name="taskTime">The provided task's time we set and count from</param>
        public void StartNextTask(TimeSpan taskTime)
        {
            // Set provided time
            CurrentTaskTimeLeft = taskTime;

            // Start the timer
            mSecondsTicker.Start();
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Runs every time the timer ticks
        /// </summary>
        private void SecondsTicker_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Add one second to the session duration
            SessionDuration += TimeSpan.FromSeconds(1);

            // Substract one second from the task time
            CurrentTaskTimeLeft -= TimeSpan.FromSeconds(1);

            // If the task finished already...
            if (CurrentTaskTimeLeft <= TimeSpan.FromSeconds(0))
            {
                // Inform everyone about it
                TaskFinished.Invoke();

                // Stop the timer
                mSecondsTicker.Stop();
            }
        }

        #endregion
    }
}
