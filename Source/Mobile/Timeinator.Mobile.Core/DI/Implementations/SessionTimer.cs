﻿using System;
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
        /// One second duration TimeSpan
        /// </summary>
        private static readonly TimeSpan OneSecond = TimeSpan.FromSeconds(1);

        /// <summary>
        /// The timer that elapses every second so everything related to time can update everytime it ticks
        /// </summary>
        private Timer mSecondsTicker;

        /// <summary>
        /// Indicates if break-mode is on or off
        /// </summary>
        private bool mIsOnBreak;

        #endregion

        #region Public Properties

        /// <summary>
        /// The duration of the whole current session
        /// </summary>
        public TimeSpan SessionDuration { get; private set; }

        /// <summary>
        /// The duration of recent task
        /// </summary>
        public TimeSpan TaskDuration { get; private set; }

        /// <summary>
        /// The time that left in current session task
        /// </summary>
        public TimeSpan CurrentTaskTimeLeft { get; private set; }

        /// <summary>
        /// The duration of current break
        /// </summary>
        public TimeSpan CurrentBreakDuration { get; set; }

        #endregion

        #region Public Events

        /// <summary>
        /// The event that is fired any time the current task finishes
        /// </summary>
        public event Action TaskFinished = () => { };

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Setups new session in the timer
        /// </summary>
        /// <param name="timerAction">The action to fire along with timer ticks</param>
        /// <param name="taskAction">The action to fire when task's time finishes</param>
        public void SetupSession(Action timerAction, Action taskAction)
        {
            // Reset session
            SessionDuration = TimeSpan.Zero;
            TaskDuration = TimeSpan.Zero;

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
            // Set session as not paused
            mIsOnBreak = false;

            // Reset duration
            TaskDuration = TimeSpan.Zero;

            // Set provided time
            CurrentTaskTimeLeft = taskTime;

            // Start the timer
            mSecondsTicker.Start();
        }

        /// <summary>
        /// Starts the break time on current task
        /// </summary>
        public void StartBreak()
        {
            // Erase any previous break time 
            CurrentBreakDuration = TimeSpan.Zero;

            // Set the indicator
            mIsOnBreak = true;
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Runs every time the timer ticks
        /// </summary>
        private void SecondsTicker_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Add one second to the session duration
            SessionDuration += OneSecond;

            // If break time is on...
            if (mIsOnBreak)
            {
                // Add one second to the break duration
                CurrentBreakDuration += OneSecond;

                // Don't do anything else while on break
                return;
            }

            // Substract one second from the task time
            CurrentTaskTimeLeft -= OneSecond;
            TaskDuration += OneSecond;

            // If the task finished already...
            if (CurrentTaskTimeLeft <= TimeSpan.Zero)
            {
                // Stop the timer
                mSecondsTicker.Stop();

                // Inform everyone about it
                TaskFinished.Invoke();
            }
        }

        #endregion
    }
}
