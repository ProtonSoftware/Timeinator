using System;
using System.Collections.Generic;
using System.Linq;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The service that mediates between database and <see cref="TimeTasksManager"/> to handle tasks
    /// </summary>
    public class TimeTasksService : ITimeTasksService
    {
        #region Private Members

        private readonly TimeTasksMapper mTimeTasksMapper;
        private readonly ITimeTasksManager mTimeTasksManager;
        private readonly ITimeTasksRepository mTimeTasksRepository;
        private readonly IUserTimeHandler mUserTimeHandler;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        public TimeTasksService(ITimeTasksManager timeTasksManager, ITimeTasksRepository timeTasksRepository, IUserTimeHandler userTimeHandler, TimeTasksMapper tasksMapper)
        {
            // Get injected DI services
            mTimeTasksManager = timeTasksManager;
            mTimeTasksRepository = timeTasksRepository;
            mUserTimeHandler = userTimeHandler;
            mTimeTasksMapper = tasksMapper;
        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Switches orderId of given context
        /// </summary>
        /// <param name="contexts">Current list of contexts</param>
        /// <param name="swap">Context to change order of</param>
        /// <param name="newid">New position of swap context</param>
        /// <returns>Reordered list</returns>
        public List<TimeTaskContext> SwitchOrder(List<TimeTaskContext> contexts, TimeTaskContext swap, int newid)
        {
            contexts.Find(x => x == swap).OrderId = newid;
            contexts = contexts.OrderBy(x => x.OrderId).ToList();
            var orig = contexts.FindIndex(x => x.OrderId == newid);
            for (var i = orig; i < contexts.Count; i++)
            {
                if (contexts[i] != swap)
                    contexts[i].OrderId++;
            }
            return contexts;
        }

        /// <summary>
        /// Loads saved tasks from the database and passes it in the manager
        /// </summary>
        /// <returns>A list of found tasks mapped as context</returns>
        public List<TimeTaskContext> LoadStoredTasks()
        {
            // Prepare a list to return
            var taskContexts = new List<TimeTaskContext>();


            // Get every task in the database
            var dbTasks = mTimeTasksRepository.GetSavedTasksForToday();

            // For each of them...
            foreach (var entity in dbTasks)
            {
                // Map it as a context
                var context = mTimeTasksMapper.Map(entity);

                // Add it to the list
                taskContexts.Add(context);
            }

            // Return every found task
            return taskContexts;
        }

        /// <summary>
        /// Sets up the manager and transfers specified tasks
        /// </summary>
        /// <param name="tasks">The tasks that user wants to have in the session</param>
        /// <param name="userTime">The time user has declared to calculate</param>
        public void ConveyTasksToManager(List<TimeTaskContext> tasks, TimeSpan userTime)
        {
            // Add the list to the manager with provided time
            mTimeTasksManager.UploadTasksList(SetTaskOrder(tasks), userTime);
        }

        /// <summary>
        /// Sets up the time handler and starts new session
        /// </summary>
        /// <param name="tasks">The tasks that user wants to have in the session</param>
        public void ConveyTasksToTimeHandler(List<TimeTaskContext> tasks)
        {
            // Add the list to the time handler and start it
            mUserTimeHandler.StartTimeHandler(tasks);
        }

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
            // Prepare a list of task ids to remove
            var taskIds = new List<int>();

            // For each of provided tasks...
            foreach (var task in contexts)
            {
                // Immortal tasks won't be removed
                if (task.IsImmortal)
                    continue;

                // Add the id to the list
                taskIds.Add(task.Id);
            }

            // Send collected ids to the repository to remove associated tasks
            mTimeTasksRepository.RemoveTasks(taskIds);
        }

        #endregion

        #region Private Helpers

        /// <summary>
        /// Sets first time OrderId to offer user basic order
        /// </summary>
        /// <returns>Task list with correct OrderId</returns>
        private List<TimeTaskContext> SetTaskOrder(List<TimeTaskContext> rawContexts)
        {
            List<TimeTaskContext> rawImportant = TaskListHelpers.GetImportant(rawContexts),
                rawSimple = TaskListHelpers.GetImportant(rawContexts, true);
            rawImportant = rawImportant.OrderBy(x => x.Priority).Reverse().ToList();
            rawSimple = rawSimple.OrderBy(x => x.Priority).Reverse().ToList();
            var oid = 0;
            for (var i = 0; i < rawImportant.Count; i++)
            {
                rawImportant[i].OrderId = oid;
                oid++;
            }
            for (var i = 0; i < rawSimple.Count; i++)
            {
                rawSimple[i].OrderId = oid;
                oid++;
            }
            return rawImportant.Concat(rawSimple).ToList();
        }

        #endregion
    }
}
