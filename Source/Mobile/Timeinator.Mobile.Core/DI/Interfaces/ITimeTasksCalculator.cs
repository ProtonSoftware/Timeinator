using System;
using System.Collections.Generic;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The interface for time task calculator
    /// </summary>
    public interface ITimeTasksCalculator
    {
        TimeSpan CalculateMinimumTimeForTasks(List<TimeTaskContext> contexts);

        List<TimeTaskContext> CalculateTasksForSession(List<TimeTaskContext> contexts, TimeSpan sessionTime);
        List<TimeTaskContext> CalculateTasksAfterResume(List<TimeTaskContext> contexts, TimeSpan remainingTime);
    }
}
