namespace Timeinator.Mobile
{
    /// <summary>
    /// The service that mediates between database and <see cref="TimeTasksManager"/> to handle tasks
    /// </summary>
    public class TimeTasksService : ITimeTasksService
    {
        /// <summary>
        /// Loads saved tasks from the database and passes it in the manager
        /// </summary>
        /// <returns>True, if any task was found, false otherwise</returns>
        public bool LoadCurrentTasks()
        {
            // Get every task in the database
            var dbTasks = DI.TimeTasksRepository.GetSavedTasksForToday();

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
    }
}
