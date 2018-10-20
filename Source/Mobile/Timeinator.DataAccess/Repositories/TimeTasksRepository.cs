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

        #endregion
    }
}
