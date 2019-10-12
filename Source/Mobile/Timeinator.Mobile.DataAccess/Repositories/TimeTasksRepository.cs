using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Timeinator.Mobile.DataAccess
{
    /// <summary>
    /// The repository that handles database access for saving and getting time tasks
    /// </summary>
    public class TimeTasksRepository : BaseRepository<TimeTask, int>, ITimeTasksRepository
    {
        #region Protected Properties

        /// <summary>
        /// The table in database that holds time tasks that is used to access them
        /// </summary>
        protected override DbSet<TimeTask> DbSet => Db.TimeTasks;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database context for this application</param>
        public TimeTasksRepository(TimeinatorMobileDbContext dbContext) : base(dbContext) { }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Gets every time task that is saved in the database
        /// </summary>
        /// <param name="queryString">The filter text for tasks</param>
        /// <returns>List of time task entities</returns>
        public IEnumerable<TimeTask> GetSavedTasksForToday(string queryString)
        {
            // Get every saved task from the database
            var result = GetAll();

            // If we have a query string to filter tasks...
            if (!string.IsNullOrWhiteSpace(queryString))
            {
                // Get every word from the query separately
                foreach (var word in queryString.Split(' '))
                {
                    // Skip empty words
                    if (string.IsNullOrWhiteSpace(word))
                        continue;

                    // Filter the tasks for that specified word
                    result = result.Where(x => x.Name.Contains(word) || x.TagsString.Contains(word));
                }
            }

            // Return the result tasks
            return result;
        }

        /// <summary>
        /// Saves specified task entity to the database
        /// </summary>
        /// <param name="entity">Task entity</param>
        public void SaveTask(TimeTask entity)
        {
            // Get whats in database already
            var dbEntity = DbSet.Where(x => x.Id == entity.Id).FirstOrDefault();

            // If there is no task with same Id
            if (dbEntity == null)
                // Add new entity to the database
                DbSet.Add(entity);

            // Otherwise...
            else
            {
                // Set every property for new values
                dbEntity.Name = entity.Name;
                dbEntity.Description = entity.Description;
                dbEntity.Type = entity.Type;
                dbEntity.IsImmortal = entity.IsImmortal;
                dbEntity.IsImportant = entity.IsImportant;
                dbEntity.Priority = entity.Priority;
                dbEntity.Progress = entity.Progress;
                dbEntity.MaxProgress = entity.MaxProgress;
                dbEntity.Tags = entity.Tags;
                dbEntity.AssignedTime = entity.AssignedTime;
                dbEntity.CreationDate = entity.CreationDate;
            }

            // Save the changes we made
            SaveChanges();
        }

        /// <summary>
        /// Removes specified tasks from the database by provided ids
        /// </summary>
        /// <param name="ids">The ids of tasks to remove</param>
        public void RemoveTasks(IEnumerable<int> ids)
        {
            // Get list of every entity associated with provided ids
            var entities = DbSet.Where(x => ids.Contains(x.Id));

            // Remove the list from database
            DbSet.RemoveRange(entities);

            // Save changes made
            SaveChanges();
        }

        #endregion
    }
}
