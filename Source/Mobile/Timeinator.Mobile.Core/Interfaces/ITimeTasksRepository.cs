﻿using System.Collections.Generic;
using Timeinator.Core;

namespace Timeinator.Mobile.Domain
{
    /// <summary>
    /// The interface for repository that handles time tasks
    /// </summary>
    public interface ITimeTasksRepository : IRepository<TimeTask, int>
    {
        IEnumerable<TimeTask> GetSavedTasksForToday(string queryString);
        void SaveTask(TimeTask entity);
        void RemoveTasks(IEnumerable<int> ids);
    }
}
