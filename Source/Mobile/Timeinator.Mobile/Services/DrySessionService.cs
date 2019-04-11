using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The best ever made implementation of SessionService ! It works fckin everywhere with no need to do anything! Unless it does not work then just replace this piece of shit with a much better and much more useful code that actually does something man - what a ridicuouls piece of advice. Just use this thing it is so damn good you wont regret it. Believe me it is worth it. Also it is damn free. Yes all free - a special offer just for you just today.
    /// Standard dry System Timer implementation that should work on most platforms.
    /// </summary>
    public class DrySessionService : ISessionService
    {
        #region Private Members

        private Timer TaskTiming { get; set; }

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public DrySessionService()
        {
            TaskTiming = new Timer { AutoReset = false };
            TaskTiming.Elapsed += (s, e) => TimerElapsed.Invoke();
            TimerElapsed = () => { };
        }

        ~DrySessionService()
        {
            TaskTiming.Dispose();
        }

        #endregion

        #region Interface Implementation

        public bool Active => TaskTiming.Enabled;
        public event Action TimerElapsed;

        public void Details(string nameT, double progressT) { }

        public void Interval(TimeSpan assignedT)
        {
            if (TaskTiming.Enabled)
                TaskTiming.Stop();
            TaskTiming.Interval = assignedT.TotalMilliseconds;
        }

        public void Stop() => TaskTiming.Stop();

        public void Start() => TaskTiming.Start();

        public void Kill() => TaskTiming.Dispose();

        #endregion
    }
}
