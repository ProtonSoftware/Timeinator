using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Timeinator.Core;

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

        /// <summary>
        /// Gets all currently saved settings in the database
        /// </summary>
        /// <returns>List of every setting as <see cref="SettingsPropertyInfo"/></returns>
        public List<SettingsPropertyInfo> GetAllSettings()
        {
            // Prepare a list of property infos to return
            var propertyList = new List<SettingsPropertyInfo>();

            // Get all the settings from database
            var entities = DbSet.ToList();

            // For each one...
            foreach (var entity in entities)
            {
                // Create new property info
                var propertyInfo = new SettingsPropertyInfo
                {
                    Name = entity.Name,
                    Type = Type.GetType(entity.Type),
                    Value = entity.Value
                };

                // Add it to the list
                propertyList.Add(propertyInfo);
            }

            // Return every collected setting
            return propertyList;
        }

        /// <summary>
        /// Saves specified settin's new value into database
        /// </summary>
        /// <param name="setting">The setting that value got changed</param>
        public void SaveSetting(SettingsPropertyInfo setting)
        {
            // Get the setting based on its name
            var entity = DbSet.Where(x => x.Name == setting.Name).FirstOrDefault();

            // If none was found
            if (entity == null)
            {
                // Create new one
                entity = new Setting
                {
                    Name = setting.Name,
                    Type = setting.Type.ToString()
                };

                // And add it to the database
                DbSet.Add(entity);
            }

            // Change its value
            entity.Value = setting.Value.ToString();

            // Save changes we made
            SaveChanges();
        }

        #endregion
    }
}
