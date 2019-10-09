using System;
using System.Collections.Generic;
using System.Linq;
using Timeinator.Core;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.Core
{
    /// <summary>
    /// The service that handles all the logic associated with tasks in database interaction
    /// </summary>
    public class TimeTasksService : ITimeTasksService
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksRepository mTimeTasksRepository;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTasksService(ITimeTasksRepository timeTasksRepository, TimeTasksMapper tasksMapper)
        {
            // Get injected DI services
            mTimeTasksRepository = timeTasksRepository;
            mTimeTasksMapper = tasksMapper;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Saves new task to the database and adds it to the application's task list
        /// </summary>
        /// <param name="context">The context of a task to add</param>
        public void SaveTask(TimeTaskContext context)
        {
            // Map it to the entity
            var entity = mTimeTasksMapper.ReverseMap(context);

            // Save it into database
            mTimeTasksRepository.SaveTask(entity);
        }

        /// <summary>
        /// Removes single specified task from the database
        /// </summary>
        /// <param name="context">The task to delete</param>
        public void RemoveTask(TimeTaskContext context)
        {
            // Prepare an one element list with provided task's id
            var list = new List<int> { context.Id };

            // Send it to the repository to delete this task
            mTimeTasksRepository.RemoveTasks(list);
        }

        /// <summary>
        /// Removes finished tasks from the database
        /// Ommits "immortal" tasks
        /// </summary>
        public void RemoveFinishedTasks(List<TimeTaskContext> contexts)
        {
            // Get list of task ids...
            var taskIds = contexts
                         // That are not immortal
                         .Where(x => x.IsImmortal == false)
                         // Take ids instead of tasks
                         .Select(x => x.Id);

            // Send collected ids to the repository to remove associated tasks
            mTimeTasksRepository.RemoveTasks(taskIds);
        }

        /// <summary>
        /// Loads every saved task from the database
        /// </summary>
        /// <param name="queryString">The filter text for tasks</param>
        /// <returns>A list of found tasks as <see cref="TimeTaskContext"/></returns>
        public List<TimeTaskContext> LoadStoredTasks(string queryString)
        {
            // Get every task in the database
            var dbTasks = mTimeTasksRepository.GetSavedTasksForToday(queryString);

            // Map entities as contexts
            var result = mTimeTasksMapper.ListMap(dbTasks.ToList());

            // Return every found task
            return result;
        }

        #endregion
    }
}
