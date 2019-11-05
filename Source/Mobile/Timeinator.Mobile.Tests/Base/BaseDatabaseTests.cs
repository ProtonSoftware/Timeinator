using Timeinator.Mobile.DataAccess;
using System;
using Microsoft.EntityFrameworkCore;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The base class for every database tester
    /// </summary>
    public class BaseDatabaseTests : IDisposable
    {
        #region Setup

        /// <summary>
        /// The database that is used in the application
        /// </summary>
        protected TestTimeinatorMobileDbContext DatabaseContext { get; set; }

        /// <summary>
        /// Default constructor
        /// </summary>
        public BaseDatabaseTests()
        {
            // Create the database
            DatabaseContext = new TestTimeinatorMobileDbContext();

            // Make sure its created properly
            DatabaseContext.Database.EnsureCreated();
        }

        /// <summary>
        /// Dispose after every test
        /// </summary>
        public void Dispose()
        {
            // Make sure database is deleted
            DatabaseContext.Database.EnsureDeleted();

            // Dispose the database
            DatabaseContext.Dispose();
        }

        #endregion
    }
}
