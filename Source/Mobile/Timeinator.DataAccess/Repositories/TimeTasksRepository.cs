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
        public TimeTasksRepository(TimeinatorMobileDbContext dbContext) : base(dbContext)
        {

        }

        #endregion

        #region Interface Implementation

        /// <summary>
        /// Gets every time task that is saved in the database
        /// </summary>
        /// <returns>List of time task entities</returns>
        public List<TimeTask> GetSavedTasksForToday()
        {
            var result = DbSet.Where(x => x.Id > 0);

            return result.ToList();
        }

        /// <summary>
        /// Saves specified task entity to the database
        /// </summary>
        /// <param name="entity">Task entity</param>
        public void SaveTask(TimeTask entity)
        {
            // Add entity to the database
            DbSet.Add(entity);

            // Save it
            SaveChanges();
        }

        /// <summary>
        /// Removes specified tasks from the database by provided ids
        /// </summary>
        /// <param name="ids">The ids of tasks to remove</param>
        public void RemoveTasks(List<int> ids)
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
