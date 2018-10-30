using System.Collections.Generic;

namespace Timeinator.Mobile
{
    /// <summary>
    /// The service that mediates between database and <see cref="TimeTasksManager"/> to handle tasks
    /// </summary>
    public class TimeTasksService : ITimeTasksService
    {
        #region Interface Implementation

        /// <summary>
        /// Loads saved tasks from the database and passes it in the manager
        /// </summary>
        /// <returns>True, if any task was found, false otherwise</returns>
        public bool LoadCurrentTasks()
        {
            // Get every task in the database
            var dbTasks = DI.TimeTasksRepository.GetSavedTasksForToday();

            // Clear current manager list to begin with
            DI.TimeTasksManager.TaskContexts = new List<TimeTaskContext>();

            // For each of them...
            foreach (var entity in dbTasks)
            {
                // Map it as a context
                var context = DI.TimeTasksMapper.Map(entity);

                // Add it to the manager
                DI.TimeTasksManager.AddTask(context);
            }

            // If we successfully loaded some tasks to the manager
            if (DI.TimeTasksManager.TaskContexts.Count > 0)
                return true;

            // Otherwise, no tasks were saved
            return false;
        }

        /// <summary>
        /// Saves new task to the database and adds it to the application's task list
        /// </summary>
        /// <param name="context">The context of a task to add</param>
        public void SaveNewTask(TimeTaskContext context)
        {
            // Map it to the entity
            var entity = DI.TimeTasksMapper.ReverseMap(context);

            // Save it into database
            DI.TimeTasksRepository.SaveTask(entity);

            // TODO: Make it different way - manager should be "refreshed" and load every task
            DI.TimeTasksManager.AddTask(context);
        }

        #endregion
    }
}
