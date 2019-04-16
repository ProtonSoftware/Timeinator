using Microsoft.EntityFrameworkCore;
using System.IO;

namespace Timeinator.Mobile.DataAccess
{
    /// <summary>
    /// The database context for this application
    /// Contains every database table as db sets
    /// </summary>
    public class TimeinatorMobileDbContext : DbContext
    {
        #region Db Sets

        /// <summary>
        /// The table for saved users' time tasks
        /// </summary>
        public DbSet<TimeTask> TimeTasks { get; set; }

        /// <summary>
        /// The table for saved application's settings
        /// </summary>
        public DbSet<Setting> Settings { get; set; }

        #endregion

        #region Database Configuration

        /// <summary>
        /// Configures SQLite database specifically for mobile application
        /// </summary>
        /// <param name="optionsBuilder">Default options builder</param>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Do default things that base class requires
            BaseOnConfiguring(optionsBuilder);

            // Configure the builder to save database locally on mobile device
            optionsBuilder.UseSqlite($"Filename={Path.Combine(System.Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "timeinatorDB.sqlite")}");
        }

        /// <summary>
        /// Base OnConfiguring method kept as a reference for contexts that will inherit current one (used in test context for unit testing)
        /// </summary>
        protected void BaseOnConfiguring(DbContextOptionsBuilder optionsBuilder) => base.OnConfiguring(optionsBuilder);

        #endregion
    }
}
