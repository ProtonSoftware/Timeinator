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
        /// <returns>A list of found tasks mapped as context</returns>
        public List<TimeTaskContext> LoadStoredTasks()
        {
            // Prepare a list to return
            var taskContexts = new List<TimeTaskContext>();

            // Get every task in the database
            var dbTasks = DI.TimeTasksRepository.GetSavedTasksForToday();

            // For each of them...
            foreach (var entity in dbTasks)
            {
                // Map it as a context
                var context = DI.TimeTasksMapper.Map(entity);

                // Add it to the list
                taskContexts.Add(context);
            }

            // Return every found task
            return taskContexts;
        }

        /// <summary>
        /// Sets up the manager and transfers specified tasks
        /// </summary>
        /// <param name="tasks">The task that user wants to have in the session</param>
        public void ConveyTasksToManager(List<TimeTaskContext> tasks)
        {
            // Add the list to the manager
            DI.TimeTasksManager.UploadTasksList(tasks);
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
        }

        #endregion
    }
}
