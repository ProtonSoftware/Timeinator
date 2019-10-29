using Microsoft.EntityFrameworkCore;
using SimpleInjector;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.Android
{
    /// <summary>
    /// Dependency Injection container for this application
    /// </summary>
    public static class DI
    {
        #region Public Properties

        /// <summary>
        /// The underlying container from which the dependencies are retrieved
        /// </summary>
        public static Container Container { get; set; }

        #endregion

        #region Public Methods

        /// <summary>
        /// Creates the database and applies all the migrations
        /// </summary>
        public static void MigrateDatabase()
        {
            // Migrate the database
            Container.GetInstance<TimeinatorMobileDbContext>().Database.Migrate();
        }

        #endregion
    }
}
