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
    }
}
