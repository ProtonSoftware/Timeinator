using Microsoft.EntityFrameworkCore;
using System;
using Timeinator.Mobile.DataAccess;

namespace Timeinator.Mobile.Tests
{
    /// <summary>
    /// The database context for this application
    /// Contains every database table as db sets
    /// </summary>
    public class TestTimeinatorMobileDbContext : TimeinatorMobileDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            // Do default things that base class requires
            BaseOnConfiguring(optionsBuilder);

            optionsBuilder.UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString());
        }
    }
}
