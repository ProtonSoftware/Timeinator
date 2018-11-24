using Microsoft.EntityFrameworkCore;

namespace Timeinator.Mobile.DataAccess
{
    /// <summary>
    /// The repository for saving application settings in database
    /// </summary>
    public class SettingsRepository : BaseRepository<Setting, int>, ISettingsRepository
    {
        #region Protected Properties

        /// <summary>
        /// The table in database that holds every application's setting values
        /// </summary>
        protected override DbSet<Setting> DbSet => Db.Settings;

        #endregion

        #region Constructor

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="dbContext">The database context for this application</param>
        public SettingsRepository(TimeinatorMobileDbContext dbContext) : base(dbContext)
        {

        }

        #endregion

        #region Interface Implementation



        #endregion
    }
}
