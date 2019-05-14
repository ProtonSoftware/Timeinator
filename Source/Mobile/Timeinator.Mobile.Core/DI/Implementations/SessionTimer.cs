using System;
using System.Timers;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The timer implementation for task session
    /// </summary>
    public class SessionTimer : ISessionTimer
    {
        #region Public Properties

        /// <summary>
        /// The timer that elapses every second so everything related to time can update everytime it ticks
        /// </summary>
        public Timer SecondsTicker { get; set; }

        /// <summary>
        /// The duration of the whole current session
        /// </summary>
        public TimeSpan SessionDuration { get; private set; } = new TimeSpan(0);

        #endregion

        public void StartSession(Action action)
        {
            // Timer setup
            SecondsTicker = new Timer(1000);
            SecondsTicker.Elapsed += SecondsTicker_Elapsed;
            SecondsTicker.Elapsed += (s, e) => action();

            // Start the timer
            SecondsTicker.Start();
        }

        private void SecondsTicker_Elapsed(object sender, ElapsedEventArgs e)
        {
            // Add one second to the session duration
            SessionDuration = SessionDuration.Add(new TimeSpan(0, 0, 1));
        }
    }
}
